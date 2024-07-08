using System.Net;
using System.Net.Http.Headers;
using mArI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace mArI.Extensions;

public static class AddMariExtensions
{
    public static void AddMari(this IServiceCollection servicesCollection, string openAiApiKey)
    {
        servicesCollection.AddHttpClient("mArIOpenApiClientInternal", client =>
        {
            client.BaseAddress = new("https://api.openai.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        });

        servicesCollection.AddTransient<Government>();
        servicesCollection.AddTransient<OpenAiHttpService>();
    }
}