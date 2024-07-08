using System.Text.Json.Serialization;

namespace mArI.Models;

public class ImageFileDetail 
{
    [JsonPropertyName("file_id")]
    public string FileId {get;set;}

    [JsonPropertyName("detail")]
    public string Detail {get;set;}
}