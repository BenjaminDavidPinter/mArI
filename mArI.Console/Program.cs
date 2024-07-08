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

ColorConsole.Write("System Input: ", fgColor: ConsoleColor.Yellow);
var question = Console.ReadLine();
try
{

    await testGov.GenerateCommittee(
    //NOTE: Committee Name
    "TestCommittee",
    //NOTE: Committee Setup Prompt
    ["You are an assistant which inspects employment documents. Your specific job is to read the entire document, and determine for what year the document validates. Look for words like 'Check Date' or 'Begin'. In your reponse, only return the year, and absolutley no other text",
    "You are an assistant which inspects employment documents. Your job is to determine what year the document's information refers to. In your reponse, only return the year, and absolutley no other text."],
    //NOTE: Number of members of this committee
    5);

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
        ColorConsole.WriteLine($"  |_{answer.AssistantInfo.Id} - [{answer.AssistantInfo.Name}] - [{string.Join("", answer.AssistantInfo.Instructions.Take(50))}]", fgColor: ConsoleColor.White);
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