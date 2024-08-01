using mArI.Lib.Models;
using mArI.Models;

namespace mArI.Services;

public class OpenAIAssistantService(OpenAiHttpService httpService)
{
    /// <summary>
    /// Create an Open Ai Assistant
    /// </summary>
    /// <param name="assistantToCreate"></param>
    /// <returns></returns>
    public async Task<Assistant<ResponseFormatType>> CreateAssistant<ResponseFormatType>(Assistant<ResponseFormatType> assistantToCreate){
        return await httpService.CreateAssistant(assistantToCreate);
    }

    /// <summary>
    /// Upload a list of files to OpenAI
    /// </summary>
    /// <param name="files">The key is the filename, the value is the file content</param>
    /// <returns></returns>
    public async Task<List<OpenAiFile>> UploadFiles(Dictionary<string, byte[]> files, string purpose){
        List<OpenAiFile> uploadedFiles = [];
        foreach(var key in files.Keys)
        {
            uploadedFiles.Add(await httpService.UploadFile(files[key], key, purpose));
        }
        return uploadedFiles;
    }

    /// <summary>
    /// Create vector store
    /// </summary>
    /// <param name="vectoreStoreToCreate"></param>
    /// <returns></returns>
    public async Task<VectorStore> CreateVectorStore(VectorStore vectoreStoreToCreate) 
    {
        return await httpService.CreateVectorStore(vectoreStoreToCreate);
    }

    /// <summary>
    /// Attach uploaded files to the vector store
    /// </summary>
    /// <param name="vectorStore"></param>
    /// <param name="filesToAttach"></param>
    /// <returns></returns>
    public async Task<List<VectorStoreFile>> AddFilesToVectorStore(VectorStore vectorStore, List<OpenAiFile> filesToAttach)
    {
        List<VectorStoreFile> filesThatWereAttached = [];
        foreach (var file in filesToAttach)
        {
            filesThatWereAttached.Add(await httpService.CreateVectorStoreFile(vectorStore.Id, file.Id));
        }
        return filesThatWereAttached;
    }

    /// <summary>
    /// Attempt to wait for all files requested for upload are finished
    /// </summary>
    /// <param name="storeToWait"></param>
    /// <param name="pollingRate"></param>
    /// <param name="maxTries"></param>
    /// <returns></returns>
    public async Task WaitForVectorStoreUploadsToFinish(VectorStore storeToWait, int? pollingRate = 1000, int? maxTries = 5)
    {
        storeToWait = await httpService.GetVectorStore(storeToWait.Id);
        int totalTries = 0;
        while(storeToWait.FileCounts.InProgress > 0 && totalTries <= maxTries){
            await Task.Delay(pollingRate.Value);
            storeToWait = await httpService.GetVectorStore(storeToWait.Id);
            totalTries += 1;
        }
        if(totalTries == maxTries){
            throw new Exception("Files did not finish uploading in time.");
        }
    }

    /// <summary>
    /// Attach a vector store to an assistant
    /// </summary>
    /// <param name="assistant"></param>
    /// <param name="store"></param>
    /// <returns></returns>
    public async Task<Assistant<object>> AddVectorStoreToAssistant(Assistant<object> assistant, VectorStore store) {
        if(assistant.ToolResources == null){
            assistant.ToolResources = new();
            assistant.ToolResources.FileSearch = new();
        }
        if(assistant.ToolResources.FileSearch == null){
            assistant.ToolResources.FileSearch = new();
        }
        if(assistant.ToolResources.FileSearch.VectorStoreIds == null){
            assistant.ToolResources.FileSearch.VectorStoreIds = new();
        }

        assistant.ToolResources.FileSearch.VectorStoreIds.Add(store.Id);
        return await httpService.ModifyAssistant(assistant);
    }


    /// <summary>
    /// Remove a vector store from an assistant
    /// </summary>
    /// <param name="assistant"></param>
    /// <param name="store"></param>
    /// <returns></returns>
    public async Task<Assistant<object>> RemoveVectorStoreFromAssistant(Assistant<object> assistant, VectorStore store){
        assistant.ToolResources.FileSearch.VectorStoreIds = assistant.ToolResources.FileSearch.VectorStoreIds.Where(x => x != store.Id).ToList();
        return await httpService.ModifyAssistant(assistant);
    }

    /// <summary>
    /// Ask a question to an assistant
    /// </summary>
    /// <param name="message"></param>
    /// <param name="assistant"></param>
    /// <returns></returns>
    public async Task<List<MessageContent>> AskQuestionToAssistant<T>(Message<string> message, Assistant<T> assistant) {
        List<MessageContent> resultMessages = [];

        var targetThread = await httpService.CreateThread();
        await httpService.CreateMessage(targetThread.Id, message);
        var run = await httpService.CreateRun(targetThread.Id, assistant.Id);
        var completedRun = await WaitForRunToComplete(targetThread.Id, run.Id);
        var runSteps = await httpService.ListRunSteps(targetThread.Id, completedRun.Id);
        foreach (var step in runSteps.Steps)
        {
            var thisMessage = await httpService.GetMessage(targetThread.Id, step.StepDetails.MessageCreation.MessageId);
            resultMessages.AddRange(thisMessage.Content);
        }
        return resultMessages;
    }

    /// <summary>
    /// Delete specified assistant
    /// </summary>
    /// <param name="assistantToDelete"></param>
    /// <returns></returns>
    public async Task<DeleteObjectResponse> DeleteAssistant(Assistant<object> assistantToDelete){
        return await httpService.DeleteAssistant(assistantToDelete.Id);
    }

    /// <summary>
    /// Method to pause execution until a question is answered
    /// </summary>
    /// <param name="threadId"></param>
    /// <param name="runId"></param>
    /// <param name="pollingRate"></param>
    /// <returns></returns>
    private async Task<Run> WaitForRunToComplete(string threadId, string runId, int? pollingRate = 1000)
    {
        var currentResult = await httpService.GetRun(threadId, runId);
        while (currentResult.Status != "completed"
        && currentResult.Status != "failed"
        && currentResult.Status != "incomplete"
        && currentResult.Status != "cancelled"
        && currentResult.Status != "requires_action"
        && currentResult.Status != "expired")
        {
            await Task.Delay(pollingRate.Value);
            currentResult = await httpService.GetRun(threadId, runId);
        }

        return currentResult;
    }
}