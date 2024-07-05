using mArI.Models;

namespace mArI.Services;

public class Government
{
    private OpenAiHttpService openAiHttpService { get; set; }
    private Dictionary<string, List<Assistant>> Committees { get; set; }

    public Government(OpenAiHttpService httpService)
    {
        Committees = new();
        openAiHttpService = httpService;
    }

    public async Task<List<Assistant>> AddCommitteeMember(string committeeName, params Assistant[] assistants)
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

    public async Task<Message<List<MessageContent>>> CreateMessage(string threadId, Message<string> message)
    {
        return await openAiHttpService.CreateMessage(threadId, message);
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
}