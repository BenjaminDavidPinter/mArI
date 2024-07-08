using System.Net.Http.Headers;
using Moq;
using mArI.Services;
using System.Xml;



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

ColorConsole.Write("Enter Two names split by a pipe: ", fgColor: ConsoleColor.Yellow);
var question = Console.ReadLine();
try
{

    await testGov.GenerateCommittee(
    //NOTE: Committee Name
    "TestCommittee",
    //NOTE: Committee Setup Prompt
    ["You are an assistant for comparing two full names, and determining" +
    "\r\n whether they could potentially refer to the same person. You will be presented with two full names, separated by" +
    "\r\n pipe characters. " +
    "\r\n  1. If the names have similar first names, and a shared last name, they should match." +
    "\r\n  2. If one of the names includes a middle name, but the other does not, ignore the middle name for matching." +
    "\r\n  2. Take into consideration variations and nicknames for the first and middle name." +
    "\r\nReturn only 'true' or 'false' depending on if they could a varation of the same name for" +
    "\r\n one another.",
    //NOTE: Various flavors of prompt text
    "You are an assistant for comparing two full persons names, and determining" +
    "\r\n whether they refer to the same person. You will be presented with two full names, separated by" +
    "\r\n pipe characters. " +
    "\r\n  1. If the names have first names which are the same, or are nicknames of one another, and a shared last name, they should match." +
    "\r\n  2. If one of the names includes a middle name, but the other does not, ignore the middle name for matching. If they both have a middle name, consider them in your decision" +
    "\r\n  2. Take into consideration variations and nicknames for the first and middle name." +
    "\r\nReturn only 'true' or 'false' depending on if they could a varation of the same name for" +
    "\r\n one another."],
    //NOTE: Number of members of this committee
    20);

    var committeeAnswer = await testGov.AskQuestionToCommittee("TestCommittee", question);

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
        ColorConsole.WriteLine($"  |_{answer.AssistantInfo.Id} - [{answer.AssistantInfo.Name}] - [{string.Join("",answer.AssistantInfo.Instructions.Take(50))}]", fgColor: ConsoleColor.White);
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