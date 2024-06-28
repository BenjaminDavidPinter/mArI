using System.ComponentModel;
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
        var responseObject = httpClient.PostAsync("assistants", JsonContent.Create<Assistant>(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<List<Assistant>> ListAssistants()
    {
        var responseObject = httpClient.GetAsync("assistants");

        throw new NotImplementedException();
    }

    public async Task<Assistant> GetAssistant(string assistantId)
    {
        var responseObject = httpClient.GetAsync($"assistants/{assistantId}");

        throw new NotImplementedException();
    }

    public async Task<Assistant> ModifyAssistant(Assistant createAssistantRequest)
    {
        var responseObject = httpClient.PostAsync("assistants", JsonContent.Create<Assistant>(createAssistantRequest
        , new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json)
        , System.Text.Json.JsonSerializerOptions.Default));

        throw new NotImplementedException();
    }

    public async Task<DeleteAssistantResponse> DeleteAssistant(string assistantId)
    {
        var responseObject = httpClient.DeleteAsync($"assistants/{assistantId}");

        throw new NotImplementedException();
    }
}