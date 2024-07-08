using System.Net.Http.Headers;
using Moq;
using mArI.Services;
using System.Xml;



ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var testClient = new HttpClient();
var openAiApiKey = File.ReadAllText(@"/Users/benjaminpinter/ApiKey.txt").Trim(); //NOTE: DO NOT CHECK IN THE API KEY
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

var question = Console.ReadLine();
try
{
    await testGov.GenerateCommittee("TestCommittee",
    "You are an assistant for comparing two names, and determining" +
    "\r\n whether they could refer to the same person. You will be presented with two names, separated by" +
    "\r\n pipe characters. " +
    "  1. If the names have similar first names, and a shared last name, they should match." +
    "  2. If one of the names includes a middle name, but the other does not, ignore the middle name for matching." +
    "  2. Take into consideration variations and nicknames for the first and middle name." +
    "Return only 'true' or 'false' depending on if they could a varation of the same name for" +
    "\r\n one another.",
    10);

    var committeeAnswer = await testGov.AskQuestionToCommittee("TestCommittee",
    question);

    Console.WriteLine();
    ColorConsole.WriteLine("~Committee Results~", fgColor: ConsoleColor.Blue);
    Console.WriteLine();
    ColorConsole.WriteLine($"Total Members: {committeeAnswer.Count}", fgColor: ConsoleColor.White);
    Console.WriteLine();
    ColorConsole.WriteLine($"Committee Setup: \r\nYou are an assistant for comparing two names, and determining" +
    "\r\n whether they could refer to the same person. You will be presented with two names, separated by" +
    "\r\n pipe characters. Return only 'true' or 'false' depending on if they could a varation of the same name for" +
    "\r\n one another.", fgColor: ConsoleColor.White);
    Console.WriteLine();
    ColorConsole.WriteLine($"Committee Question: \r\n{question}", fgColor: ConsoleColor.White);
    Console.WriteLine();
    foreach (var answer in committeeAnswer)
    {
        ColorConsole.WriteLine($"{answer.RunInfo.Id}", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"|_{answer.ThreadInfo.Id}", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"  |_{answer.AssistantInfo.Id} - [{answer.AssistantInfo.Name}]", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"    |_{answer.Answer.Id}", fgColor: ConsoleColor.White);
        ColorConsole.Write($"      |_", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"{answer.GetAnswerAsText()}", fgColor: ConsoleColor.Green);
    }

    var answerGroupings = committeeAnswer.GroupBy(x => x.GetAnswerAsText());

    Console.WriteLine();
    ColorConsole.WriteLine("~Result~", fgColor: ConsoleColor.Blue);
    foreach (var group in answerGroupings)
    {
        ColorConsole.Write($"{group.Key} - ", fgColor: ConsoleColor.White);
        ColorConsole.WriteLine($"{group.Count()}", fgColor: ConsoleColor.Cyan);
    }

    await testGov.DestroyAssistants();


}
catch (Exception e)
{
    Console.WriteLine();
    ColorConsole.Write("X", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($" - {e.Message}", fgColor: ConsoleColor.Red);
}