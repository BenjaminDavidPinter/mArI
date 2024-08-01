using Moq;
using mArI.Services;
using mArI.Models.Enums;
using System.Text.Json;
using mArI.Models;


var openAiApiKey = File.ReadAllText(@"C:\vs\ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);


var myAssistant = await assistantService.CreateAssistant<ResponseFormat>(new(OpenAiModel.GPT4o){
    ResponseFormat = new(){Type = "json_object"},
    Temperature = 0.01
});
var result = await assistantService.AskQuestionToAssistant(new Message<string>()
{
    Role = "user",
    Content = "What color is the sky? Return the answer as a json object, specifying only the color"
}, myAssistant);

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() {WriteIndented = true}));