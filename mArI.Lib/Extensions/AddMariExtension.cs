using System.Net;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;
using mArI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace mArI.Extensions;

public static class AddMariExtensions
{
    public static void AddMari(this IServiceCollection servicesCollection, 
        string openAiApiKey, 
        int maxRequestsPerSecond)
    {
        servicesCollection.AddTransient<Government>();
        servicesCollection.AddSingleton(new OpenAiHttpService(openAiApiKey, maxRequestsPerSecond));
        servicesCollection.AddTransient<OpenAIAssistantService>();
    }
}