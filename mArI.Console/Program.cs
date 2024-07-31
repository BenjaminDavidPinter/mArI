using Moq;
using mArI.Services;
using mArI.Lib.Models;
using mArI.Models;
using mArI.Model;
using System.Linq.Expressions;


#region Setup
ColorConsole.WriteLine("Setup Phase", fgColor: ConsoleColor.Blue);
var openAiApiKey = File.ReadAllText(@"").Trim();
var factoryMoq = new Mock<IHttpClientFactory>();
OpenAiHttpService testServ = new(openAiApiKey, 10000);

ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine(" - Moq Setup", fgColor: ConsoleColor.White);

var myAssistant = await testServ.CreateAssistant(new("gpt-4o")
{
});
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Created Assistant ({myAssistant.Id})", fgColor: ConsoleColor.White);

var uploadedFile = await testServ.UploadFile(File.ReadAllBytes(@""), "NC_Rule_Out_3-21-19.docx", FilePurposes.Assistants);
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Created Simple File ({uploadedFile.FileName}=>{uploadedFile.Id})", fgColor: ConsoleColor.White);

var newVectorStore = await testServ.CreateVectorStore(new()
{
    Name = "Test Vector Store"
});
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Created Vector Store ({newVectorStore.Name}=>{newVectorStore.Id})", fgColor: ConsoleColor.White);

var newVectorStoreFile = await testServ.CreateVectorStoreFile(newVectorStore.Id, uploadedFile.Id);
ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
ColorConsole.WriteLine($" - Requested Vector Store File Creation on {newVectorStore.Id} with {newVectorStoreFile.Id})", fgColor: ConsoleColor.White);

newVectorStore = await testServ.GetVectorStore(newVectorStore.Id);
while (newVectorStore.FileCounts.InProgress > 0)
{
    ColorConsole.WriteLine("\t\tUpload In Progress...", fgColor: ConsoleColor.Yellow);
    newVectorStore = await testServ.GetVectorStore(newVectorStore.Id);
}
if (newVectorStore.FileCounts.Completed == 1)
{
    ColorConsole.WriteLine("\t\tUpload Completed", fgColor: ConsoleColor.Green);
}
else
{
    ColorConsole.WriteLine("\t\tUpload Failed", fgColor: ConsoleColor.Red);
    newVectorStoreFile = await testServ.RetrieveVectorStoreFile(newVectorStore.Id, newVectorStoreFile.Id);
    ColorConsole.WriteLine($"\t\t\t{newVectorStoreFile.LastError.Message}", fgColor: ConsoleColor.Red);
    ColorConsole.WriteLine($"\t\t\t{newVectorStoreFile.LastError.Code}", fgColor: ConsoleColor.Red);
}

#endregion

#region Teardown
ColorConsole.WriteLine("Teardown Phase", fgColor: ConsoleColor.Blue);
ListObjectResponse<Assistant> listAssistantsResponse = new();
do
{
    listAssistantsResponse = await testServ.ListAssistants();
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine($" - Got Assistants ({listAssistantsResponse.Data.Count()})", fgColor: ConsoleColor.White);

    foreach (var assistant in listAssistantsResponse.Data)
    {
        var deletionResult = await testServ.DeleteAssistant(assistant.Id);
        ColorConsole.Write("\t\t\u221A", fgColor: ConsoleColor.Green);
        ColorConsole.WriteLine($" - Deleted Assistant ({assistant.Id})", fgColor: ConsoleColor.White);
    }
} while (listAssistantsResponse.HasMore);

ListObjectResponse<OpenAiFile> listFilesResponse = new();
do
{
    listFilesResponse = await testServ.ListFiles();
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine($" - Got Files ({listFilesResponse.Data.Count()})", fgColor: ConsoleColor.White);

    foreach (var file in listFilesResponse.Data)
    {
        var deletionResult = await testServ.DeleteFile(file.Id);
        ColorConsole.Write("\t\t\u221A", fgColor: ConsoleColor.Green);
        ColorConsole.WriteLine($" - Deleted File ({file.Id})", fgColor: ConsoleColor.White);
    }
} while (listFilesResponse.HasMore);

ListObjectResponse<VectorStore> listVectorStores = new();
do
{
    listVectorStores = await testServ.ListVectorStores();
    ColorConsole.Write("\t\u221A", fgColor: ConsoleColor.Green);
    ColorConsole.WriteLine($" - Got Vector Stores ({listVectorStores.Data.Count()})", fgColor: ConsoleColor.White);

    foreach (var vectorStore in listVectorStores.Data)
    {
        ColorConsole.Write("\t\t?", fgColor: ConsoleColor.Yellow);
        ColorConsole.WriteLine($" - Deleting VectorStore ({vectorStore.Id})", fgColor: ConsoleColor.White);
        var filesInStore = await testServ.ListVectorStoreFiles(vectorStore.Id);
        foreach (var vsFile in filesInStore.Data)
        {
            var response = await testServ.DeleteVectorStoreFile(vectorStore.Id, vsFile.Id);
            ColorConsole.Write("\t\t\t\u221A", fgColor: ConsoleColor.Green);
            ColorConsole.WriteLine($" - Deleted VectoreStoreFile ({response.Id})", fgColor: ConsoleColor.White);
        }
        var deletionResult = await testServ.DeleteVectorStore(vectorStore.Id);
        ColorConsole.Write("\t\t\u221A", fgColor: ConsoleColor.Green);
        ColorConsole.WriteLine($" - Deleted VectorStore ({vectorStore.Id})", fgColor: ConsoleColor.White);
    }
} while (listVectorStores.HasMore);
#endregion



