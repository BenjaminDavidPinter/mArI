using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using mArI.Lib.Models;
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
        , new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        }));

        return await ProcessResultToObject<Assistant>(responseObject);
    }

    public async Task<ListAssistantsResponse> ListAssistants()
    {
        var responseObject = await httpClient.GetAsync("assistants");

        return await ProcessResultToObject<ListAssistantsResponse>(responseObject);
    }

    public async Task<Assistant> GetAssistant(string assistantId)
    {
        var responseObject = await httpClient.GetAsync($"assistants/{assistantId}");

        return await ProcessResultToObject<Assistant>(responseObject);
    }

    public async Task<Assistant> ModifyAssistant(Assistant createAssistantRequest)
    {
        var responseObject = await httpClient.PostAsync("assistants", JsonContent.Create(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        return await ProcessResultToObject<Assistant>(responseObject);
    }

    public async Task<DeleteAssistantResponse> DeleteAssistant(string assistantId)
    {
        var responseObject = await httpClient.DeleteAsync($"assistants/{assistantId}");

        return await ProcessResultToObject<DeleteAssistantResponse>(responseObject);
    }

    public async Task<OpenAiThread> CreateThread()
    {
        var responseObject = await httpClient.PostAsync("threads", null);

        return await ProcessResultToObject<OpenAiThread>(responseObject);
    }

    public async Task<OpenAiThread> GetThread(string threadId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}");

        return await ProcessResultToObject<OpenAiThread>(responseObject);
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

        return await ProcessResultToObject<OpenAiThread>(responseObject);
    }

    public async Task<DeleteThreadResponse> DeleteThread(string threadId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}");

        return await ProcessResultToObject<DeleteThreadResponse>(responseObject);
    }

    public async Task<Message<List<object>>> CreateMessage<T>(string threadId, Message<T> message)
    {
        var postContent = JsonContent.Create(message
            , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
            , new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/messages", postContent);

        return await ProcessResultToObject<Message<List<object>>>(responseObject);
    }

    public async Task<List<Message<MessageContent>>> ListMessages(string threadId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/messages");

        throw new NotImplementedException();
    }

    public async Task<Message<List<MessageContent>>> GetMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/messages/{messageId}");

        return await ProcessResultToObject<Message<List<MessageContent>>>(responseObject);
    }

    public async Task<Message<MessageContent>> ModifyMessage(string threadId, string messageId, Dictionary<string, string> metadata)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/messages", JsonContent.Create<Dictionary<string, string>>(metadata
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<DeleteMessageResponse> DeleteMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}/messages/{messageId}");

        return await ProcessResultToObject<DeleteMessageResponse>(responseObject);
    }

    public async Task<Run> CreateRun(string threadId, string assistantId)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/runs", JsonContent.Create(new
        {
            assistant_id = assistantId
        }
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        return await ProcessResultToObject<Run>(responseObject);
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
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/runs/{runId}");

        return await ProcessResultToObject<Run>(responseObject);
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

    public async Task<RunStepList> ListRunSteps(string threadId, string runId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/runs/{runId}/steps");

        return await ProcessResultToObject<RunStepList>(responseObject);
    }

    public async Task<RunStep> GetRunStep(string threadId, string runId, string stepId)
    {
        var responseObject = httpClient.GetAsync($"threads/{threadId}/runs/{runId}/steps/{stepId}");

        throw new NotImplementedException();
    }


    private async Task<T> ProcessResultToObject<T>(HttpResponseMessage result)
    {
        if (!result.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{result.StatusCode} - {result.ReasonPhrase}");
        }

        var requestContent = await result.Content.ReadAsStringAsync();
        var deserializedObject = JsonSerializer.Deserialize<T>(requestContent, JsonSerializerOptions.Default);
        if (deserializedObject != null)
        {
            return deserializedObject;
        }
        else
        {
            throw new Exception("Unable to deserialize result from Create Assistant");
        }
    }

    public async Task<OpenAiFile> UploadFile(byte[] bytes, string fileName)
    {
        var content = new MultipartFormDataContent
            {
                { new StringContent("vision"), "purpose" },
                { new ByteArrayContent(bytes), "file", fileName }
            };

        var response = await httpClient.PostAsync("files", content);

        return await ProcessResultToObject<OpenAiFile>(response);
    }

    public async Task<(bool status, string description)> DeleteFile(string fileId)
    {
        var deleteFileUri = $"files/{fileId}";

        var response = await httpClient.DeleteAsync(deleteFileUri);

        if (response.IsSuccessStatusCode)
        {
            return (true, "File was deleted");
        }
        else
        {
            return (false, response.ReasonPhrase! ?? string.Empty);
        }
    }

    public async Task<ListFilesResponse> ListFiles()
    {
        var listFilesUrl = "files";

        var response = await httpClient.GetAsync(listFilesUrl);

        return await ProcessResultToObject<ListFilesResponse>(response);
    }
}