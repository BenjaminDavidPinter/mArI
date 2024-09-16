using Moq;
using mArI.Services;
using mArI.Models.Enums;
using mArI.Models;


var openAiApiKey = File.ReadAllText(@"C:\vs\ApiKey.txt").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);
var assistantService = new OpenAIAssistantService(testServ);