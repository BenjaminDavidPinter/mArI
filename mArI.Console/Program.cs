using Moq;
using mArI.Services;

var openAiApiKey = File.ReadAllText(@"/Users/benjaminpinter/ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);

var answer = await assistantService.AskQuestion("Who is arguably the best water polo player of all time?");

Console.WriteLine(answer);
