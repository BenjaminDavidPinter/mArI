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

    public async Task AddCommitteeMember(string committeeName, params Assistant[] assistants)
    {
        InitializeCommitteeInDict(committeeName);
        foreach (var assist in assistants)
        {
            var newAssist = await openAiHttpService.CreateAssistant(assist);
            AddAssistantToCommittee(committeeName, newAssist);
        };
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

    public async Task<List<DeleteAssistantResponse>> Destroy()
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