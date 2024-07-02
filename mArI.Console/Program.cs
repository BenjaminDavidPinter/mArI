using System.Net.Http.Headers;
using System.Net.Http;
using Moq;
using mArI.Services;
using mArI.Models;

ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var testClient = new HttpClient();
var openAiApiKey = ""; //NOTE: DO NOT CHECK IN THE API KEY
testClient.BaseAddress = new("https://api.openai.com/v1/");
testClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);
var factoryMoq = new Mock<IHttpClientFactory>();
factoryMoq.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(testClient);
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Moq Setup", fgColor: ConsoleColor.White);

OpenAiHttpService testServ = new(factoryMoq.Object);
Government testGov = new(testServ);
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Library Setup", fgColor: ConsoleColor.White);
try
{
    await testGov.AddCommitteeMember("TestCommittee", [new("gpt-4o"), new("gpt-4o")]);
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Assistants Created", fgColor: ConsoleColor.White);

    var members = testGov.TryGetCommittee("TestCommittee");
    foreach (var member in members?? [])
    {
        ColorConsole.WriteLine($"\t\t{member.Id}", fgColor: ConsoleColor.White);
    }

    var threads = await testGov.CreateThreads(2);
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Threads Created", fgColor: ConsoleColor.White);
    foreach (var thread in threads?? [])
    {
        ColorConsole.WriteLine($"\t\t{thread.Id}", fgColor: ConsoleColor.White);
    }

    ColorConsole.WriteLine("Teardown Phase", fgColor: ConsoleColor.Blue);

    var deletedThreads = await testGov.DestroyThreads([..threads.Select(x => x.Id)]);
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Threads Destroyed", fgColor: ConsoleColor.White);
    foreach (var thread in deletedThreads?? [])
    {
        ColorConsole.WriteLine($"\t\t{thread.Id} - {thread.Deleted}", fgColor: ConsoleColor.White);
    }

    var destroyResults = await testGov.DestroyAssistants();
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Assistants destroyed", fgColor: ConsoleColor.White);
    foreach (var r in destroyResults?? [])
    {
        ColorConsole.WriteLine($"\t\t{r.Id} - {r.Deleted}", fgColor: ConsoleColor.White);
    }
}
catch (Exception e)
{
    Console.WriteLine();
    ColorConsole.Write("X", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($" - {e.Message}", fgColor: ConsoleColor.Red);
}