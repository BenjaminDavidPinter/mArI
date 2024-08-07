using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.RateLimiting;
using mArI.Lib.Models;
using mArI.Models;

namespace mArI.Services;

public class OpenAiHttpService
{
    private HttpClient httpClient { get; set; }
    public OpenAiHttpService(string apiKey, int requestsPerMinute)
    {
        var requestsPerSec = requestsPerMinute / 60;
        var options = new TokenBucketRateLimiterOptions
        {
            TokenLimit = requestsPerSec,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = requestsPerSec,
            AutoReplenishment = true
        };
        httpClient = new(handler: new ClientSideRateLimitedHandler(limiter: new TokenBucketRateLimiter(options)));
        httpClient.BaseAddress = new("https://api.openai.com/v1/");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        httpClient.Timeout = Timeout.InfiniteTimeSpan;
    }

    #region Assistant
    public async Task<Assistant<ResponseFormatType>> CreateAssistant<ResponseFormatType>(Assistant<ResponseFormatType> createAssistantRequest)
    {
        var responseObject = await httpClient.PostAsync("assistants", JsonContent.Create<Assistant<ResponseFormatType>>(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        }));

        return await ProcessResultToObject<Assistant<ResponseFormatType>>(responseObject);
    }

    public async Task<ListObjectResponse<Assistant<ResponseFormatType>>> ListAssistants<ResponseFormatType>()
    {
        var responseObject = await httpClient.GetAsync("assistants");

        return await ProcessResultToObject<ListObjectResponse<Assistant<ResponseFormatType>>>(responseObject);
    }

    public async Task<Assistant<ResponseFormatType>> GetAssistant<ResponseFormatType>(string assistantId)
    {
        var responseObject = await httpClient.GetAsync($"assistants/{assistantId}");

        return await ProcessResultToObject<Assistant<ResponseFormatType>>(responseObject);
    }

    public async Task<Assistant<ResponseFormatType>> ModifyAssistant<ResponseFormatType>(Assistant<ResponseFormatType> createAssistantRequest)
    {
        var responseObject = await httpClient.PostAsync($"assistants/{createAssistantRequest.Id}", JsonContent.Create(new
        {
            tool_resources = createAssistantRequest.ToolResources
        }
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));

        return await ProcessResultToObject<Assistant<ResponseFormatType>>(responseObject);
    }

    public async Task<DeleteObjectResponse> DeleteAssistant(string assistantId)
    {
        var responseObject = await httpClient.DeleteAsync($"assistants/{assistantId}");

        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
    }
    #endregion

    #region Thread
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
        , JsonSerializerOptions.Default));

        return await ProcessResultToObject<OpenAiThread>(responseObject);
    }

    public async Task<DeleteObjectResponse> DeleteThread(string threadId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}");

        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
    }
    #endregion

    #region Message
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

    public async Task<DeleteObjectResponse> DeleteMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}/messages/{messageId}");

        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
    }
    #endregion

    #region Run
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
    #endregion

    #region RunStep
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
    #endregion

    #region File
    public async Task<OpenAiFile> UploadFile(
        byte[] bytes, 
        string fileName,
        string purpose)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(purpose), "purpose" },
            { new ByteArrayContent(bytes), "file", fileName }
        };

        var response = await httpClient.PostAsync("files", content);

        return await ProcessResultToObject<OpenAiFile>(response);
    }

    //TODO: Turn this into a real result model
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

    public async Task<List<byte>> GetFileContent(string fileId)
    {
        var getFileContentUrl = $"files/{fileId}/content";
        var response = await httpClient.GetAsync(getFileContentUrl);
        return await ProcessResultToObject<List<byte>>(response);
    }

    public async Task<ListObjectResponse<OpenAiFile>> ListFiles()
    {
        var listFilesUrl = "files";
        var response = await httpClient.GetAsync(listFilesUrl);
        return await ProcessResultToObject<ListObjectResponse<OpenAiFile>>(response);
    }

    public async Task<OpenAiFile> RetrieveFile(string fileId)
    {
        var listFilesUrl = $"files/{fileId}";
        var response = await httpClient.GetAsync(listFilesUrl);
        return await ProcessResultToObject<OpenAiFile>(response);
    }
    #endregion

    #region Vector Stores
    public async Task<VectorStore> CreateVectorStore(VectorStore storeToCreate)
    {
        string createVectorStoreUrl = "vector_stores";
        var response = await httpClient.PostAsync(createVectorStoreUrl, JsonContent.Create(
        storeToCreate
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return await ProcessResultToObject<VectorStore>(response);
    }

    public async Task<ListObjectResponse<VectorStore>> ListVectorStores()
    {
        string listVectorStoreUrl = "vector_stores";
        var response = await httpClient.GetAsync(listVectorStoreUrl);
        return await ProcessResultToObject<ListObjectResponse<VectorStore>>(response);
    }

    public async Task<DeleteObjectResponse> DeleteVectorStore(string vectorStoreId)
    {
        string deleteVectorStoreUrl = $"vector_stores/{vectorStoreId}";
        var response = await httpClient.DeleteAsync(deleteVectorStoreUrl);
        return await ProcessResultToObject<DeleteObjectResponse>(response);
    }

    public async Task<VectorStore> GetVectorStore(string vectorStoreId)
    {
        string deleteVectorStoreUrl = $"vector_stores/{vectorStoreId}";
        var response = await httpClient.GetAsync(deleteVectorStoreUrl);
        return await ProcessResultToObject<VectorStore>(response);
    }

    public async Task<VectorStore> ModifyVectorStore(VectorStore store)
    {
        string createVectorStoreUrl = $"vector_stores/{store.Id}";
        var response = await httpClient.PostAsync(createVectorStoreUrl, JsonContent.Create(
        store
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return await ProcessResultToObject<VectorStore>(response);
    }
    #endregion

    #region Vector Store Files
    public async Task<VectorStoreFile> CreateVectorStoreFile(string vectorStoreId, string fileId)
    {
        var endpoint = $"vector_stores/{vectorStoreId}/files";
        //TODO: Should I make this an object? It's kind of small...
        var response = await httpClient.PostAsync(endpoint, JsonContent.Create(
        new
        {
            file_id = fileId
        }
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return await ProcessResultToObject<VectorStoreFile>(response);
    }

    public async Task<VectorStoreFile> RetrieveVectorStoreFile(string vectorStoreId, string fileId)
    {
        var endpoint = $"vector_stores/{vectorStoreId}/files/{fileId}";
        //TODO: Should I make this an object? It's kind of small...
        var response = await httpClient.GetAsync(endpoint);
        return await ProcessResultToObject<VectorStoreFile>(response);
    }

    public async Task<ListObjectResponse<VectorStore>> ListVectorStoreFiles(string vectorStoreId)
    {
        var endpoint = $"vector_stores/{vectorStoreId}/files";
        //TODO: Should I make this an object? It's kind of small...
        var response = await httpClient.GetAsync(endpoint);
        return await ProcessResultToObject<ListObjectResponse<VectorStore>>(response);
    }

    public async Task<DeleteObjectResponse> DeleteVectorStoreFile(string vectorStoreId, string fileId)
    {
        var endpoint = $"vector_stores/{vectorStoreId}/files/{fileId}";
        //TODO: Should I make this an object? It's kind of small...
        var response = await httpClient.DeleteAsync(endpoint);
        return await ProcessResultToObject<DeleteObjectResponse>(response);
    }


    #endregion

    #region Internal
    private async Task<T> ProcessResultToObject<T>(HttpResponseMessage result)
    {
        if (!result.IsSuccessStatusCode)
        {
            var responseContent = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{result.ReasonPhrase} - {responseContent}");
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
    #endregion

}