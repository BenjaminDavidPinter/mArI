using System.Text.Json.Serialization;

namespace mArI.Models;

public class Attachment 
{
    [JsonPropertyName("file_id")]
    public string? FileId {get;set;}

    [JsonPropertyName("tools")]
    public List<Tool>? Tools {get;set;}
}