using mArI.Models;
using System.Text.Json.Serialization;

namespace mArI.Lib.Models;

public class ListAssistantsResponse
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("data")]
    public List<Assistant>? Data { get; set; }

    [JsonPropertyName("first_id")]
    public string? FirstId { get; set; }
    
    [JsonPropertyName("last_id")]
    public string? LastId { get; set; }
    
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
