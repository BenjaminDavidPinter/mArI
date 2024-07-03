using System.Text.Json.Serialization;

namespace mArI.Models;

public class DeleteMessageResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}