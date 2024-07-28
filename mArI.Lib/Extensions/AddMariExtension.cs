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
    public static void AddMari(this IServiceCollection servicesCollection, string openAiApiKey)
    {
        servicesCollection.AddHttpClient("mArIOpenApiClientInternal", client =>
        {
            client.BaseAddress = new("https://api.openai.com/v1/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        });
        servicesCollection.AddRateLimiter(_ =>
        _.AddTokenBucketLimiter(policyName: "Rate Limiter", options =>
        {
            options.TokenLimit = 50;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 10;
            options.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            options.TokensPerPeriod = 50;
            options.AutoReplenishment = true;
        }));
        servicesCollection.AddTransient<Government>();
        servicesCollection.AddTransient<OpenAiHttpService>();
    }
}