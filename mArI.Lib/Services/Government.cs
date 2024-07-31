using System.ComponentModel;
using mArI.Lib.Models;
using mArI.Model;
using mArI.Models;
using Microsoft.VisualBasic;

namespace mArI.Services;

public class Government
{
    private OpenAiHttpService openAiHttpService { get; set; }

    //NOTE: Should Committees be objects?
    private Dictionary<string, List<Assistant>> Committees { get; set; }
    private Dictionary<string, List<OpenAiFile>> CommitteeFiles { get; set; }

    public Government(OpenAiHttpService httpService)
    {
        Committees = [];
        CommitteeFiles = [];
        openAiHttpService = httpService;
    }

    /// <summary>
    /// Uploads a file to OpenAI, for Assistant vision.
    /// </summary>
    /// <param name="filesToUpload"></param>
    /// <param name="owningCommittee"></param>
    /// <returns>A list of all files associated with the given committee</returns>
    public void UploadFiles(List<string> owningCommittees, List<byte[]> filesToUpload)
    {
        List<Task<OpenAiFile>> fileUploadTasks = [];

        foreach (var fileToUpload in filesToUpload)
        {
            fileUploadTasks.Add(openAiHttpService.UploadFile(fileToUpload, $"{Guid.NewGuid()}.jpg", FilePurposes.Vision));
        }


        Task.WaitAll([.. fileUploadTasks]);

        foreach (var uploadedFileResult in fileUploadTasks)
        {
            foreach (var owningCommittee in owningCommittees)
            {
                if (CommitteeFiles.ContainsKey(owningCommittee))
                {
                    CommitteeFiles[owningCommittee].Add(uploadedFileResult.Result);
                }
                else
                {
                    CommitteeFiles.Add(owningCommittee, [uploadedFileResult.Result]);
                }
            }
        }
    }

    /// <summary>
    /// Create a committee
    /// </summary>
    /// <param name="committeeName"></param>
    /// <param name="model"></param>
    /// <param name="instructions"></param>
    /// <param name="committeeSize"></param>
    /// <param name="minTopP"></param>
    /// <param name="maxTopP"></param>
    /// <param name="minTemp"></param>
    /// <param name="maxTemp"></param>
    /// <returns>A list containing a reference to each member of the committee</returns>
    public async Task<List<Assistant>> GenerateCommittee(
        string committeeName,
        string model,
        string[] instructions,
        int committeeSize,
        double minTopP = 0.00,
        double maxTopP = 0.99,
        double minTemp = 0.00,
        double maxTemp = 0.99)
    {
        List<Assistant> assistants = [];

        //Pre-configure all the assistants in memory
        //Split all incoming prompts as evenly as possible
        var totalInstructions = instructions.Count();
        int assistantSplit = committeeSize / totalInstructions;

        for (int j = 0; j < instructions.Count(); j++)
        {
            for (int i = 0; i < assistantSplit; i++)
            {
                assistants.Add(new(model)
                {
                    TopP = GetPseudoDoubleWithinRange(minTopP, maxTopP),
                    Temperature = GetPseudoDoubleWithinRange(minTemp, maxTemp),
                    Name = $"{Environment.MachineName}_{j:00}_{i:0000}",
                    Instructions = instructions[j]
                });
            }
        }

        if (assistants.Count < committeeSize)
        {
            assistants.Add(new(model)
            {
                TopP = GetPseudoDoubleWithinRange(minTopP, maxTopP),
                Temperature = GetPseudoDoubleWithinRange(minTemp, maxTemp),
                Name = $"{Environment.MachineName}_S",
                Instructions = instructions[new Random().Next(0, instructions.Count())]
            });
        }

        return await AddCommitteeMember(committeeName, [.. assistants]);
    }

    //TODO: Needs refactoring
    public async Task<List<CommitteeAnswer>> AskQuestionToCommittee(string committeeName, string question)
    {
        if (!TryGetCommittee(committeeName, out var members))
        {
            throw new Exception($"Could not find committee '{committeeName}'");
        }

        CommitteeFiles.TryGetValue(committeeName, out var files);
        var threads = await CreateThreads(members.Count);

        List<Task<Message<List<object>>>> messageCreationTasks = [];
        foreach (OpenAiThread t in threads)
        {
            if (files != null && files.Count() != 0)
            {
                List<ImageFile> filesToUpload = [];
                foreach (var file in files)
                {
                    filesToUpload.Add(new ImageFile() { Type = "image_file", FileDetails = new() { FileId = file.Id, Detail = "high" } });
                }
                Message<List<object>> message = new()
                {
                    Role = "user",
                    Content = [
                        new MessageText() {Type = "text", Text = question},
                        ..filesToUpload
                    ]
                };
                messageCreationTasks.Add(CreateMessageWithFile(t.Id, message));
            }
            else
            {
                Message<string> message = new()
                {
                    Role = "user",
                    Content = question
                };
                messageCreationTasks.Add(CreateMessage(t.Id, message));
            }
        }
        Task.WaitAll([.. messageCreationTasks]);
        List<Message<List<object>>> messages = new();
        foreach (var createdMessage in messageCreationTasks)
        {
            messages.Add(createdMessage.Result);
        }

        List<Run> runs = new();
        List<Task<Run>> runCreationTasks = [];
        List<string> usedAssistants = [];
        foreach (Message<List<object>> m in messages)
        {
            var targetAssistant = members.First(x => !usedAssistants.Contains(x.Id ?? throw new Exception("Missing Id on OpenAI Object")));
            usedAssistants.Add(targetAssistant.Id);
            runCreationTasks.Add(CreateRun(m.ThreadId, targetAssistant.Id));
        }
        Task.WaitAll([.. runCreationTasks]);
        foreach (var createdRun in runCreationTasks)
        {
            runs.Add(createdRun.Result);
        }

        List<Task<Run>> pendingRuns = [];
        foreach (var run in runs)
        {
            pendingRuns.Add(WaitForRunToComplete(run.ThreadId, run.Id));
        }
        Task.WaitAll([.. pendingRuns]);

        List<Task<RunStepList>> runStepListsTasks = [];
        foreach (var run in runs)
        {
            runStepListsTasks.Add(GetRunSteps(run.ThreadId, run.Id));
        }
        Task.WaitAll([.. runStepListsTasks]);

        List<Task<Message<List<MessageContent>>>> aiMessages = [];
        foreach (var aiMessage in runStepListsTasks)
        {
            foreach (var step in aiMessage.Result.Steps)
            {
                aiMessages.Add(GetMessage(step.ThreadId, step.StepDetails.MessageCreation.MessageId));
            }
        }
        Task.WaitAll([.. aiMessages]);

        List<CommitteeAnswer> answers = [];
        foreach (var answer in aiMessages)
        {
            var targetThread = threads.Where(x => x.Id == answer.Result.ThreadId).First();
            var targetRun = runs.Where(x => x.Id == answer.Result.RunId).First();
            var targetAssistant = members.Where(x => x.Id == answer.Result.AssistantId).First();

            answers.Add(new()
            {
                Answer = answer.Result,
                ThreadInfo = targetThread,
                RunInfo = targetRun,
                AssistantInfo = targetAssistant
            });
        }

        return answers;
    }

    private async Task<List<Assistant>> AddCommitteeMember(string committeeName, params Assistant[] assistants)
    {
        InitializeCommitteeInDict(committeeName);
        List<Task<Assistant>> assistantCreationTasks = [];
        foreach (var assist in assistants)
        {
            assistantCreationTasks.Add(openAiHttpService.CreateAssistant(assist));
        };

        Task.WaitAll([.. assistantCreationTasks]);

        foreach (var createdAssistant in assistantCreationTasks)
        {
            AddAssistantToCommittee(committeeName, createdAssistant.Result);
        }

        return Committees[committeeName];
    }

    public async Task<List<OpenAiThread>> CreateThreads(int count)
    {
        List<OpenAiThread> threads = [];
        List<Task<OpenAiThread>> threadTasks = [];
        for (int i = 0; i < count; i++)
        {
            threadTasks.Add(openAiHttpService.CreateThread());
        }

        Task.WaitAll([.. threadTasks]);

        foreach (var createdThread in threadTasks)
        {
            threads.Add(createdThread.Result);
        }

        return threads;
    }

    public async Task<List<DeleteObjectResponse>> DestroyThreads(params string[] threadIds)
    {
        List<DeleteObjectResponse> responses = [];
        List<Task<DeleteObjectResponse>> deleteThreadRequests = [];
        foreach (var threadId in threadIds)
        {
            deleteThreadRequests.Add(openAiHttpService.DeleteThread(threadId));
        }

        Task.WaitAll([.. deleteThreadRequests]);

        foreach (var deletedThread in deleteThreadRequests)
        {
            responses.Add(deletedThread.Result);
        }

        return responses;
    }

    public async Task<Message<List<object>>> CreateMessage(string threadId, Message<string> message)
    {
        return await openAiHttpService.CreateMessage(threadId, message);
    }

    public async Task<Message<List<object>>> CreateMessageWithFile(string threadId, Message<List<object>> message)
    {
        return await openAiHttpService.CreateMessage(threadId, message);
    }

    public async Task<Message<List<MessageContent>>> GetMessage(string threadId, string messageId)
    {
        return await openAiHttpService.GetMessage(threadId, messageId);
    }

    public async Task<DeleteObjectResponse> DestroyMessage(string threadId, string messageId)
    {
        return await openAiHttpService.DeleteMessage(threadId, messageId);
    }

    public bool TryGetCommittee(string committeeName, out List<Assistant> committee)
    {
        if (Committees.ContainsKey(committeeName))
        {
            committee = Committees[committeeName];
            return true;
        }

        committee = [];
        return false;
    }

    public async Task Destroy()
    {
        List<DeleteObjectResponse> responses = new();
        List<Task<DeleteObjectResponse>> deletionRequests = [];
        List<Task> fileDeletionRequests = [];
        foreach (var key in Committees.Keys)
        {
            foreach (var assist in Committees[key])
            {
                deletionRequests.Add(openAiHttpService.DeleteAssistant(assist.Id));
            }
        }

        foreach (var key in CommitteeFiles.Keys)
        {
            foreach (var file in CommitteeFiles[key])
            {
                fileDeletionRequests.Add(openAiHttpService.DeleteFile(file.Id));
            }
        }

        CommitteeFiles.Clear();
        Committees.Clear();

        Task.WaitAll([.. deletionRequests, .. fileDeletionRequests]);

        foreach (var deletionRequest in deletionRequests)
        {
            responses.Add(deletionRequest.Result);
        }
    }

    public async Task<int> DestroyAllAssistants()
    {
        var allAssistants = await openAiHttpService.ListAssistants();
        int totalDeletions = 0;
        try
        {
            List<Task<DeleteObjectResponse>> deletionRequests = [];
            foreach (var assistant in allAssistants.Data ?? new())
            {
                deletionRequests.Add(openAiHttpService.DeleteAssistant(assistant.Id));
                await Task.Delay(100);
            }

            Task.WaitAll([.. deletionRequests]);
            totalDeletions += deletionRequests.Count(x => x.Result.Deleted);

            if (allAssistants.HasMore)
            {
                totalDeletions += await DestroyAllAssistants();
            }

            return totalDeletions;
        }
        catch
        {
            return totalDeletions;
        }
    }

    public async Task<int> DestroyAllFiles()
    {
        var allFiles = await openAiHttpService.ListFiles();
        int totalDeletions = 0;
        try
        {
            List<Task<(bool status, string description)>> deletionRequests = [];
            foreach (var file in allFiles.Data ?? new())
            {
                deletionRequests.Add(openAiHttpService.DeleteFile(file.Id));
                await Task.Delay(100);
            }

            Task.WaitAll([.. deletionRequests]);
            totalDeletions += deletionRequests.Count(x => x.Result.status);

            if (allFiles.HasMore)
            {
                totalDeletions += await DestroyAllAssistants();
            }

            return totalDeletions;
        }
        catch
        {
            return totalDeletions;
        }
    }

    public async Task<Run> CreateRun(string threadId, string assistantId)
    {
        return await openAiHttpService.CreateRun(threadId, assistantId);
    }

    public async Task<Run> WaitForRunToComplete(string threadId, string runId)
    {
        var currentResult = await openAiHttpService.GetRun(threadId, runId);
        while (currentResult.Status != "completed"
        && currentResult.Status != "failed"
        && currentResult.Status != "incomplete"
        && currentResult.Status != "cancelled"
        && currentResult.Status != "requires_action"
        && currentResult.Status != "expired")
        {
            await Task.Delay(1000);
            currentResult = await openAiHttpService.GetRun(threadId, runId);
        }

        return currentResult;
    }

    public async Task<RunStepList> GetRunSteps(string threadId, string runId)
    {
        return await openAiHttpService.ListRunSteps(threadId, runId);
    }

    private void InitializeCommitteeInDict(string committeeName)
    {
        if (!Committees.ContainsKey(committeeName))
        {
            Committees.Add(committeeName, []);
        }
    }

    private void AddAssistantToCommittee(string committeeName, Assistant assist)
    {
        if (Committees.ContainsKey(committeeName))
        {
            Committees[committeeName].Add(assist);
        }
        else
        {
            Committees.Add(committeeName, [assist]);
        }
    }

    private static double GetPseudoDoubleWithinRange(double lowerBound, double upperBound)
    {
        var random = new Random();
        var rDouble = random.NextDouble();
        var rRangeDouble = rDouble * (upperBound - lowerBound) + lowerBound;
        return rRangeDouble;
    }
}