using Moq;
using mArI.Services;
using mArI.Models.Enums;
using System.Text.Json;
using mArI.Models;
using mArI.Model;
using System.CodeDom;
using System.Collections.Immutable;
using System.Reflection;


var openAiApiKey = File.ReadAllText(@"C:\vs\ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);
var gov = new Government(testServ);


var myAssistant = await assistantService.CreateAssistant<string>(new(OpenAiModel.GPT4o)
{
    Instructions = "You are an assistant which validates Employment documents. " +
                                    "Your job is to review documents which are supposed to validate that the uploader worked at a specific location. " +
                                    "You will be provided the Subject's Name, and the Employer's Name, in that order, split by a pipe character" +
                                    "You must always return a JSON object with the following information" +
                                    "'YearOfDocument' -> This is the year which the document is for" +
                                    "'EmployerOnDocument' -> This is the employer which this document verifies" +
                                    "'TypeOfDocument' -> The type of document" +
                                    "'PrimarySubjectName' -> The name of the primary subject on the document" +
                                    "'SubjectNameSimilar -> a true or false value if the Primary subject name resembles the provided subject name." +
                                    "'EmployerNameSimilar' -> a true or false value if the Employer name name on the document resembles the provided employer name (you can take subsidiary relationships into account)." +
                                    "'ImportantDates' -> A dictionary of any date found on the document, and the label for that date",
    Temperature = 0.01,
    Tools = new() { new Tool() { type = "file_search"} }
});

var QuestionAnalysisAssistant = await assistantService.CreateAssistant<string>(new(OpenAiModel.GPT4o)
{
    Instructions = "You are an assistant which evaluates a body of text provided to a person." +
                    "The text contains deliverables for validating employment periods." +
                    "I will provide you with two peices of information; The body of text which asks for the deliverables, and a list of years, split by a pipe" +
                    "Your job is to evaluate the body of text, and decide which years from the list must be validated." +
                    "For example, the body of text might include the phrase 'only the most recent year of employment', and my list of years might be [2013, 2014, 2015]. Only 2015 would need to be evaluated." +
                    "You will respond with an json object which contains an array of the years which must be validated from the provided list (Property name 'Years'), and an array of the acceptable document types specified by the body of text",
    Temperature = 0.01,
    Tools = new() { new Tool() { type = "file_search" } }
});

var myFile = await assistantService.UploadFiles(
    new Dictionary<string, byte[]>(){
        { "test.pdf", File.ReadAllBytes(@"C:\Users\bpinter\Downloads\144994133 (1).pdf") } 
    },
    FilePurposes.Assistants
);

var result = await assistantService.AskQuestionToAssistant(new Message<string>()
{
    Role = "user",
    Content = "Sean Gilbert|Utopia",
    Attachments = new()
    {
        new()
        {
            FileId = myFile.First().Id,
            Tools = new() 
            {
                new() {type = "file_search" }
            }
        }
    }
}, myAssistant);

var result2 = await assistantService.AskQuestionToAssistant(new Message<string>()
{
    Role = "user",
    Content = "Please provide documentation(W2, DD214, 1099) for the last 4 years of employment|[2013,2014,2015,2016,2017,2018]",
}, QuestionAnalysisAssistant);

var answerText = result.First().Text.Value;
var answerText2 = result2.First().Text.Value;

var answer = answerText.Substring(answerText.IndexOf('{'), answerText.LastIndexOf('}') - answerText.IndexOf('{') + 1);
var answer2 = answerText2.Substring(answerText2.IndexOf('{'), answerText2.LastIndexOf('}') - answerText2.IndexOf('{') + 1);

await gov.DestroyAllAssistants();
await gov.DestroyAllFiles();

var deser = JsonSerializer.Deserialize<DocumentAnalysisResult>(answer);
var deser2 = JsonSerializer.Deserialize<QuestionAnalysisResult>(answer2);

Console.WriteLine(answer);
Console.WriteLine(answer2);


public class DocumentAnalysisResult
{
    /*
    "YearOfDocument": 2023,
"EmployerOnDocument": "UTOPIA TECH LLC",
"TypeOfDocument": "Form 1099-NEC",
"PrimarySubjectName": "SHAUN GILBERT",
"SubjectNameSimilar": true,
"EmployerNameSimilar": true
    */
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
    /*
    "YearOfDocument": 2023,
"EmployerOnDocument": "UTOPIA TECH LLC",
"TypeOfDocument": "Form 1099-NEC",
"PrimarySubjectName": "SHAUN GILBERT",
"SubjectNameSimilar": true,
"EmployerNameSimilar": true
    */
    public List<int> Years { get; set; }
    public List<string> DocumentTypes { get; set; }
}