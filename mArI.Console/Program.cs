using System.Net.Http.Headers;
using Moq;
using mArI.Services;



ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var testClient = new HttpClient();
var openAiApiKey = File.ReadAllText(@"/Users/benjaminpinter/apikey.txt").Trim();
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

var assistantCleanup = await testGov.DestroyAllAssistants();
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Stranded Assistants Destroyed ({assistantCleanup})", fgColor: ConsoleColor.White);

var fileCleanup = await testGov.DestroyAllFiles();
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Stranded Files Destroyed ({fileCleanup})", fgColor: ConsoleColor.White);


ColorConsole.Write("Committee Prompt: ", fgColor: ConsoleColor.Yellow);
var prompt = Console.ReadLine();
ColorConsole.Write("System Input: ", fgColor: ConsoleColor.Yellow);
var question = Console.ReadLine();
try
{
    await testGov.GenerateCommittee("TestCommittee", "gpt-4o", [prompt], 21);
    testGov.UploadFiles(["TestCommittee"], new() { File.ReadAllBytes(@"C:\Users\bpinter\Downloads\TestDoc.png") });
    var committeeAnswer = await testGov.AskQuestionToCommittee("TestCommittee", question);
    Console.WriteLine();
    ColorConsole.WriteLine("~Committee Results~", fgColor: ConsoleColor.Blue);
    Console.WriteLine();
    ColorConsole.WriteLine($"Total Members: {committeeAnswer.Count}", fgColor: ConsoleColor.White);
    Console.WriteLine();
    ColorConsole.WriteLine($"Committee Question: \r\n{question}", fgColor: ConsoleColor.White);
    Console.WriteLine();
    foreach (var answer in committeeAnswer)
    {
        ColorConsole.WriteLine($"{answer.RunInfo.Id}", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"|_{answer.ThreadInfo.Id}", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"  |_{answer.AssistantInfo.Id} - [{answer.AssistantInfo.Name}] -" +
            $"[{string.Join("", answer.AssistantInfo.Instructions.Take(50))}]", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"    |_{answer.Answer.Id}", fgColor: ConsoleColor.White);
        ColorConsole.Write($"      |_", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"{answer.GetAnswerAsText()}", fgColor: ConsoleColor.Green);
    }

    var answerGroupings = committeeAnswer.GroupBy(x => x.GetAnswerAsText());

    Console.WriteLine();
    ColorConsole.WriteLine("~Result~", fgColor: ConsoleColor.Blue);
    Console.WriteLine();

    foreach (var group in answerGroupings.OrderByDescending(x => x.Count()))
    {
        ColorConsole.Write($"{group.Key} - ", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"{group.Count()}", fgColor: ConsoleColor.Cyan);
    }

    await testGov.Destroy();


}
catch (Exception e)
{
    Console.WriteLine();
    ColorConsole.Write("X", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($" - {e.Message}", fgColor: ConsoleColor.Red);
}
