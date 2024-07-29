using System.Net.Http.Headers;
using Moq;
using mArI.Services;
using mArI.Lib.Models;
using System.Threading.RateLimiting;
using mArI.Models;
using System.Runtime.Serialization;


var options = new TokenBucketRateLimiterOptions
{
    TokenLimit = 50,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    QueueLimit = 10,
    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
    TokensPerPeriod = 50,
    AutoReplenishment = true
};

ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var openAiApiKey = File.ReadAllText(@"C:\vs\ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10);

ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Moq Setup", fgColor: ConsoleColor.White);

var myAssistant = await testServ.CreateAssistant(new("gpt-4o"));
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Simple Assistant ({myAssistant.Id})", fgColor: ConsoleColor.White);

ListAssistantsResponse listAssistantsResponse = new();
do
{
    listAssistantsResponse = await testServ.ListAssistants();
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine($" - Got All Assistants ({listAssistantsResponse.Data.Count()})", fgColor: ConsoleColor.White);

    foreach (var assistant in listAssistantsResponse.Data)
    {
        var deletionResult = await testServ.DeleteAssistant(assistant.Id);
        ColorConsole.Write("\t\t\u221A", fgColor: ConsoleColor.Green);
        ColorConsole.WriteLine($" - Deleted Assistant ({assistant.Id})", fgColor: ConsoleColor.White);
    }
} while (listAssistantsResponse.HasMore);





