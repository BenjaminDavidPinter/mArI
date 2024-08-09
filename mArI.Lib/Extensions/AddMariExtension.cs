using mArI.Services;
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