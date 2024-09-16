using Moq;
using mArI.Services;
using mArI.Models.Enums;
using mArI.Models;


var openAiApiKey = File.ReadAllText(@"/Users/benjaminpinter/ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);

var myAssistant = await assistantService.CreateAssistant(new Assistant<object>(OpenAiModel.GPT4o)
{
    Instructions = "You are an assistant which answers boolean questions. You may only responsd to everything you are asked with 'true' or 'false"
});

var thisMessage = new Message<string>()
{
    Role = "user",
    Content = "Is the sky blue",
    Attachments = new()
};

var answer = await assistantService.AskQuestionToAssistant(thisMessage, myAssistant);

Console.WriteLine(answer?.First()?.Text?.Value ?? string.Empty);
