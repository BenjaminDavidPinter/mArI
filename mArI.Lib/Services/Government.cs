using System.ComponentModel;
using mArI.Models;

namespace mArI.Services;

public class Government
{
    private OpenAiHttpService openAiHttpService { get; set; }

    //NOTE: Should Committees be objects?
    private Dictionary<string, List<Assistant>> Committees { get; set; }
    private List<FileUploadResult> Files { get; set; }

    public Government(OpenAiHttpService httpService)
    {
        Committees = [];
        Files = [];
        openAiHttpService = httpService;
    }

    public async Task<List<Assistant>> GenerateCommittee(
        string committeeName,
        string[] instructions,
        int committeeSize,
        List<byte[]> files = null!,
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

        List<Task<FileUploadResult>> fileUploadTasks = [];
        if (files != null)
        {
            foreach (var fileToUpload in files)
            {
                fileUploadTasks.Add(openAiHttpService.UploadFile(fileToUpload, $"{Guid.NewGuid()}.jpg"));
            }
        }

        for (int j = 0; j < instructions.Count(); j++)
        {
            for (int i = 0; i < assistantSplit; i++)
            {
                assistants.Add(new("gpt-4o")
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
            assistants.Add(new("gpt-4o")
            {
                TopP = GetPseudoDoubleWithinRange(minTopP, maxTopP),
                Temperature = GetPseudoDoubleWithinRange(minTemp, maxTemp),
                Name = $"{Environment.MachineName}_S",
                Instructions = instructions[new Random().Next(0, instructions.Count())]
            });
        }

        //Create in parallel
        Task.WaitAll([.. fileUploadTasks]);
        Files.AddRange(fileUploadTasks.Select(x => x.Result));
        return await AddCommitteeMember(committeeName, [.. assistants]);
    }

    public async Task<List<CommitteeAnswer>> AskQuestionToCommittee(string committeeName, string question)
    {
        var members = TryGetCommittee(committeeName);
        var threads = await CreateThreads(GetRequiredThreadCount());

        List<Message<List<object>>> messages = new();
        List<Task<Message<List<object>>>> messageCreationTasks = [];
        foreach (OpenAiThread t in threads)
        {
            if (Files != null && Files.Count() != 0)
            {
                List<ImageFile> filesToUpload = [];
                foreach (var file in Files)
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
        foreach (var createdMessage in messageCreationTasks)
        {
            messages.Add(createdMessage.Result);
        }

        List<Run> runs = new();
        List<Task<Run>> runCreationTasks = [];
        List<string> usedAssistants = [];
        foreach (Message<List<object>> m in messages)
        {
            var targetAssistant = members.First(x => !usedAssistants.Contains(x.Id));
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

    public int GetRequiredThreadCount()
    {
        int total = 0;
        foreach (var key in Committees.Keys)
        {
            total += Committees[key].Count;
        }

        return total;
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

    public async Task<List<DeleteThreadResponse>> DestroyThreads(params string[] threadIds)
    {
        List<DeleteThreadResponse> responses = [];
        List<Task<DeleteThreadResponse>> deleteThreadRequests = [];
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

    public async Task<DeleteMessageResponse> DestroyMessage(string threadId, string messageId)
    {
        return await openAiHttpService.DeleteMessage(threadId, messageId);
    }

    public List<Assistant>? TryGetCommittee(string committeeName)
    {
        if (Committees.ContainsKey(committeeName))
        {
            return Committees[committeeName];
        }
        else
        {
            return null!;
        }
    }

    public async Task<List<DeleteAssistantResponse>> DestroyAssistants()
    {
        List<DeleteAssistantResponse> responses = new();
        List<Task<DeleteAssistantResponse>> deletionRequests = [];
        foreach (var key in Committees.Keys)
        {
            foreach (var assist in Committees[key])
            {
                deletionRequests.Add(openAiHttpService.DeleteAssistant(assist.Id));
            }
        }

        Task.WaitAll([.. deletionRequests]);

        foreach (var deletionRequest in deletionRequests)
        {
            responses.Add(deletionRequest.Result);
        }

        return responses;
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
            await Task.Delay(100);
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