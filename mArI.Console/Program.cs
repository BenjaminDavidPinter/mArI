using System.Net.Http.Headers;
using System.Net.Http;
using Moq;
using mArI.Services;
using mArI.Models;
using System.Diagnostics;

Stopwatch sw = new();

sw.Start();
ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var testClient = new HttpClient();
var openAiApiKey = File.ReadAllText(@"C:\VS\ApiKey.txt").Trim(); //NOTE: DO NOT CHECK IN THE API KEY
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
    await testGov.AddCommitteeMember("TestCommittee", [
        new("gpt-4o") 
        {
            Name = $"{Environment.MachineName}_{new Random().NextInt64(0, int.MaxValue)}"
        }, 
        new("gpt-4o")
        {
            Name = $"{Environment.MachineName}_{new Random().NextInt64(0, int.MaxValue)}"
        },
        new("gpt-4o")
        {
            Name = $"{Environment.MachineName}_{new Random().NextInt64(0, int.MaxValue)}"
        },
        new("gpt-4o")
        {
            Name = $"{Environment.MachineName}_{new Random().NextInt64(0, int.MaxValue)}"
        },
        new("gpt-4o")
        {
            Name = $"{Environment.MachineName}_{new Random().NextInt64(0, int.MaxValue)}"
        },
    ]);
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Assistants Created", fgColor: ConsoleColor.White);

    var members = testGov.TryGetCommittee("TestCommittee");
    foreach (var member in members?? [])
    {
        ColorConsole.WriteLine($"\t\t{member.Id}\t[{member.Name}]", fgColor: ConsoleColor.White);
    }

    var threads = await testGov.CreateThreads(members.Count);
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Threads Created", fgColor: ConsoleColor.White);
    foreach (var thread in threads?? [])
    {
        ColorConsole.WriteLine($"\t\t{thread.Id}", fgColor: ConsoleColor.White);
    }

    List<Message<List<MessageContent>>> messages = new();
    List<Task<Message<List<MessageContent>>>> messageCreationTasks = [];
    foreach(OpenAiThread t in threads) 
    {
        messageCreationTasks.Add(testGov.CreateMessage(t.Id, new() {
            Role = "user",
            Content = "What color is the sky?"
        }));
    }
    Task.WaitAll([..messageCreationTasks]);
    foreach (var createdMessage in messageCreationTasks)
    {
        messages.Add(createdMessage.Result);
    }
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Messages Created", fgColor: ConsoleColor.White);
    foreach (var message in messages?? [])
    {
        ColorConsole.WriteLine($"\t\t{message.ThreadId}", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"\t\t  {message.Id} - [{message.Content.First().Text.Value}]", fgColor: ConsoleColor.White);
    }
    sw.Stop();
    Console.WriteLine();
    ColorConsole.Write("?", fgColor: ConsoleColor.Yellow);
    ColorConsole.WriteLine($" - Completed in [{sw.Elapsed.Milliseconds}ms]", fgColor: ConsoleColor.Yellow);

    sw.Reset();
    sw.Start();
    ColorConsole.WriteLine("Teardown Phase", fgColor: ConsoleColor.Blue);

    List<DeleteMessageResponse> destroyedMessages = [];
    List<Task<DeleteMessageResponse>> destroyedMessageRequests = [];
    foreach(var m in messages){
        destroyedMessageRequests.Add(testGov.DestroyMessage(m.ThreadId, m.Id));
    }
    Task.WaitAll([..destroyedMessageRequests]);
    foreach (var destroyedMessage in destroyedMessageRequests)
    {
        destroyedMessages.Add(destroyedMessage.Result);
    }
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine(" - Messages Destroyed", fgColor: ConsoleColor.White);
    foreach (var message in destroyedMessages?? [])
    {
        ColorConsole.WriteLine($"\t\t{message.Id} - {message.Deleted}", fgColor: ConsoleColor.White);
    }

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
    Console.WriteLine();
    ColorConsole.WriteLine($" - Completed in [{sw.Elapsed.Milliseconds}ms]", fgColor: ConsoleColor.Yellow);
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine();
    ColorConsole.Write("X", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($" - {e.Message}", fgColor: ConsoleColor.Red);
}