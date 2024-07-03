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
        foreach (var assist in assistants)
        {
            var newAssist = await openAiHttpService.CreateAssistant(assist);
            AddAssistantToCommittee(committeeName, newAssist);
        };

        return Committees[committeeName];
    }

    public async Task<List<OpenAiThread>> CreateThreads(int count){
        List<OpenAiThread> threads = new();
        for(int i = 0; i < count; i++){
            threads.Add(await openAiHttpService.CreateThread());
        }

        return threads;
    }

    public async Task<List<DeleteThreadResponse>> DestroyThreads(params string[] threadIds)
    {
        List<DeleteThreadResponse> responses = [];
        foreach(var threadId in threadIds){
            responses.Add(await openAiHttpService.DeleteThread(threadId));
        }

        return responses;
    }

    public async Task<Message<List<MessageContent>>> CreateMessage(string threadId, Message<string> message)
    {
        return await openAiHttpService.CreateMessage(threadId, message);
    }

    public List<Assistant>? TryGetCommittee(string committeeName)
    {
        if(Committees.ContainsKey(committeeName)){
            return Committees[committeeName];
        }
        else {
            return null!;
        }
    }

    public async Task<List<DeleteAssistantResponse>> DestroyAssistants()
    {
        List<DeleteAssistantResponse> responses = new();
        foreach (var key in Committees.Keys)
        {
            foreach (var assist in Committees[key])
            {
                responses.Add(await openAiHttpService.DeleteAssistant(assist.Id));
            }
        }

        return responses;
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