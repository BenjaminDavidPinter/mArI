using mArI.Models;
using System.Text.Json.Serialization;

namespace mArI.Lib.Models;

public class ListObjectResponse<T>
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("data")]
    public List<T>? Data { get; set; }

    [JsonPropertyName("first_id")]
    public string? FirstId { get; set; }
    
    [JsonPropertyName("last_id")]
    public string? LastId { get; set; }
    
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
