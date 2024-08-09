using Moq;
using mArI.Services;
using mArI.Models.Enums;
using System.Text.Json;
using mArI.Models;
using mArI.Model;


var openAiApiKey = File.ReadAllText(@"C:\vs\ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);
var gov = new Government(testServ);

Console.WriteLine("What is your question: ");
var question = Console.ReadLine();


var AutomationCodeQuestionAssistant = await assistantService.CreateAssistant<string>(new(OpenAiModel.GPT4o)
{
    Instructions = "You are an assitant which answers questions asked by developer about a c-sharp project",
    Temperature = 0.01,
    Tools = new() { new Tool() { type = "file_search" } }
});

var AutomationVectorStore = await testServ.GetVectorStore("vs_BaRdMbisjDzZCnr6XjG7dfeK");

var thisMessage = new Message<string>()
{
    Role = "user",
    Content = question,
    Attachments = new()
};

await assistantService.AddVectorStoreForFileSearch(AutomationCodeQuestionAssistant, AutomationVectorStore);

var result = await assistantService.AskQuestionToAssistant(thisMessage, AutomationCodeQuestionAssistant);

var answerText = result.First().Text.Value;

Console.WriteLine(answerText);

await gov.DestroyAllAssistants();
Console.WriteLine("Nuked all Assistants");



public class DocumentAnalysisResult
{
    public int? YearOfDocument { get; set; }
    public string? EmployerOnDocument { get; set; }
    public string? TypeOfDocument { get; set; }
    public string? PrimarySubjectName { get; set; }
    public bool? SubjectNameSimilar { get; set; }
    public bool? EmployerNameSimilar { get; set; }
    public Dictionary<string, string> ImportantDates { get; set; }
}


public class QuestionAnalysisResult
{
    public List<int> Years { get; set; }
    public List<string> DocumentTypes { get; set; }
}