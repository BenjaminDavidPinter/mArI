using System.Net.Http.Headers;
using System.Net.Http;
using Moq;
using mArI.Services;
using mArI.Models;

var testClient = new HttpClient();
var openAiApiKey = "API_KEY"; //NOTE: DO NOT CHECK IN THE API KEY
testClient.BaseAddress = new("https://api.openai.com/v1/");
testClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);
var factoryMoq = new Mock<IHttpClientFactory>();
factoryMoq.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(testClient);
ColorConsole.Write("\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Moq Setup", fgColor: ConsoleColor.White);

OpenAiHttpService testServ = new(factoryMoq.Object);
Government testGov = new(testServ);
ColorConsole.Write("\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Library Setup", fgColor: ConsoleColor.White);
try
{
    await testGov.AddCommitteeMember("TestCommittee", [new("gpt-4o"), new("gpt-4o")]);
    ColorConsole.Write("\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Assistants Added", fgColor: ConsoleColor.White);

    var members = testGov.TryGetCommittee("TestCommittee");
    foreach (var member in members?? [])
    {
        ColorConsole.WriteLine($"\t{member.Id}", fgColor: ConsoleColor.White);
    }

    var destroyResults = await testGov.Destroy();
    ColorConsole.Write("\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Assistants destroyed", fgColor: ConsoleColor.White);
    foreach (var r in destroyResults?? [])
    {
        ColorConsole.WriteLine($"\t{r.Id} - {r.Deleted}", fgColor: ConsoleColor.White);
    }
}
catch (Exception e)
{
    ColorConsole.Write("X", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($" - {e.Message}", fgColor: ConsoleColor.Red);
}