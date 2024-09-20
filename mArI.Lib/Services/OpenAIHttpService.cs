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
        var responseObject = await httpClient.PostAsync("assistants", CreateStandardJsonContent(createAssistantRequest));
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

    public async Task<DeleteObjectResponse> DeleteAssistant(string assistantId)
    {
        var responseObject = await httpClient.DeleteAsync($"assistants/{assistantId}");
        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
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

    public async Task<DeleteObjectResponse> DeleteThread(string threadId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}");
        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
    }
    #endregion

    #region Message
    public async Task<Message<List<object>>> CreateMessage<T>(string threadId, Message<T> message)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/messages", CreateStandardJsonContent(message));
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

    public async Task<DeleteObjectResponse> DeleteMessage(string threadId, string messageId)
    {
        var responseObject = await httpClient.DeleteAsync($"threads/{threadId}/messages/{messageId}");
        return await ProcessResultToObject<DeleteObjectResponse>(responseObject);
    }
    #endregion

    #region Run
    public async Task<Run> CreateRun(string threadId, string assistantId)
    {
        //TODO: Make a real model here for 'CreateRunRequest'
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
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/runs");
        throw new NotImplementedException();
    }

    public async Task<Run> GetRun(string threadId, string runId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/runs/{runId}");
        return await ProcessResultToObject<Run>(responseObject);
    }

    public async Task<RunCancellationRequest> CancelRun(string threadId, string runId)
    {
        var responseObject = await httpClient.PostAsync($"threads/{threadId}/runs{runId}/cancel", null);
        throw new NotImplementedException();
    }
    #endregion

    #region RunStep
    public async Task<RunStepList> ListRunSteps(string threadId, string runId)
    {
        var responseObject = await httpClient.GetAsync($"threads/{threadId}/runs/{runId}/steps");
        return await ProcessResultToObject<RunStepList>(responseObject);
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
    public async Task<DeleteObjectResponse> DeleteFile(string fileId)
    {
        var deleteFileUri = $"files/{fileId}";
        var response = await httpClient.DeleteAsync(deleteFileUri);
        return await ProcessResultToObject<DeleteObjectResponse>(response);
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
        var response = await httpClient.PostAsync("vector_stores", CreateStandardJsonContent(storeToCreate));
        return await ProcessResultToObject<VectorStore>(response);
    }

    public async Task<ListObjectResponse<VectorStore>> ListVectorStores()
    {
        var response = await httpClient.GetAsync("vector_stores");
        return await ProcessResultToObject<ListObjectResponse<VectorStore>>(response);
    }

    public async Task<DeleteObjectResponse> DeleteVectorStore(string vectorStoreId)
    {
        var response = await httpClient.DeleteAsync($"vector_stores/{vectorStoreId}");
        return await ProcessResultToObject<DeleteObjectResponse>(response);
    }

    public async Task<VectorStore> GetVectorStore(string vectorStoreId)
    {
        var response = await httpClient.GetAsync($"vector_stores/{vectorStoreId}");
        return await ProcessResultToObject<VectorStore>(response);
    }

    public async Task<VectorStore> ModifyVectorStore(VectorStore store)
    {
        var response = await httpClient.PostAsync($"vector_stores/{store.Id}", CreateStandardJsonContent(store));
        return await ProcessResultToObject<VectorStore>(response);
    }
    #endregion

    #region Vector Store Files
    public async Task<VectorStoreFile> CreateVectorStoreFile(string vectorStoreId, string fileId)
    {
        var endpoint = $"vector_stores/{vectorStoreId}/files";
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
        var response = await httpClient.GetAsync($"vector_stores/{vectorStoreId}/files/{fileId}");
        return await ProcessResultToObject<VectorStoreFile>(response);
    }

    public async Task<ListObjectResponse<VectorStore>> ListVectorStoreFiles(string vectorStoreId)
    {
        var response = await httpClient.GetAsync($"vector_stores/{vectorStoreId}/files");
        return await ProcessResultToObject<ListObjectResponse<VectorStore>>(response);
    }

    public async Task<DeleteObjectResponse> DeleteVectorStoreFile(string vectorStoreId, string fileId)
    {
        var response = await httpClient.DeleteAsync($"vector_stores/{vectorStoreId}/files/{fileId}");
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

    private JsonContent CreateStandardJsonContent<T>(T from)
    {
        return JsonContent.Create<T>(from
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }
    #endregion

}