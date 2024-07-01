using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using mArI.Models;

namespace mArI.Services;

public class OpenAiHttpService
{
    private HttpClient httpClient { get; set; }
    public OpenAiHttpService(IHttpClientFactory httpClientFactory)
    {
        httpClient = httpClientFactory.CreateClient("mArIOpenApiClientInternal");
    }

    public async Task<Assistant> CreateAssistant(Assistant createAssistantRequest)
    {
        var responseObject = await httpClient.PostAsync("assistants", JsonContent.Create<Assistant>(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<List<Assistant>> ListAssistants()
    {
        var responseObject = await httpClient.GetAsync("assistants");

        throw new NotImplementedException();
    }

    public async Task<Assistant> GetAssistant(string assistantId)
    {
        var responseObject = await httpClient.GetAsync($"assistants/{assistantId}");

        throw new NotImplementedException();
    }

    public async Task<Assistant> ModifyAssistant(Assistant createAssistantRequest)
    {
        var responseObject = await httpClient.PostAsync("assistants", JsonContent.Create<Assistant>(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<DeleteAssistantResponse> DeleteAssistant(string assistantId)
    {
        var responseObject = await httpClient.DeleteAsync($"assistants/{assistantId}");

        throw new NotImplementedException();
    }

    public async Task<OpenAiThread> CreateThread()
    {
        var responseObject = await httpClient.PostAsync("threads", null);

        throw new NotImplementedException();
    }

    public async Task<OpenAiThread> GetThread(string threadId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}");

        throw new NotImplementedException();
    }

    public async Task<OpenAiThread> ModifyThread(string threadId, List<Tool> toolResources, Dictionary<string, string> metaData)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}", JsonContent.Create(new
        {
            tool_resources = toolResources,
            metadata = metaData
        }
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<DeleteThreadResponse> DeleteThread(string threadId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}");

        throw new NotImplementedException();
    }

    public async Task<Message> CreateMessage(string threadId, Message message)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/messages", JsonContent.Create<Message>(message
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<List<Message>> ListMessages(string threadId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/messages");

        throw new NotImplementedException();
    }

    public async Task<Message> GetMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/messages/{messageId}");

        throw new NotImplementedException();
    }

    public async Task<Message> ModifyMessage(string threadId, string messageId, Dictionary<string, string> metadata)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/messages", JsonContent.Create<Dictionary<string, string>>(metadata
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<DeleteMessageResponse> DeleteMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}/messages/{messageId}");

        throw new NotImplementedException();
    }

    public async Task<Run> CreateRun(string threadId, Run run)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/runs", JsonContent.Create<Run>(run
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<(Thread thread, Run run)> CreateThreadAndRun(string assistantId, OpenAiThread threadRequest)
    {
        var responseObject = await httpClient.PostAsync("threads/runs", JsonContent.Create(new
        {
            assistant_id = assistantId,
            thread = threadRequest
        },
        new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json),
        System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<List<Run>> ListRuns(string threadId)
    {
        var responseObject = httpClient.GetAsync($"threads/{threadId}/runs");

        throw new NotImplementedException();
    }

    public async Task<Run> GetRun(string threadId, string runId)
    {
        var responseObject = httpClient.GetAsync($"threads/{threadId}/runs/{runId}");

        throw new NotImplementedException();
    }

    public async Task<Run> ModifyRun(string threadId, string runId, Dictionary<string, string> metadata)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/runs/{runId}", JsonContent.Create<Dictionary<string, string>>(metadata
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<Run> SubmitToolOutputs(string threadId, string runId, List<ToolOutput> toolOutputs)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/runs/{runId}/submit_tool_outputs",
        JsonContent.Create<List<ToolOutput>>(toolOutputs, new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<RunCancellationRequest> CancelRun(string threadId, string runId)
    {
        var responseObject = httpClient.PostAsync($"threads/{threadId}/runs{runId}/cancel", null);

        throw new NotImplementedException();
    }
}