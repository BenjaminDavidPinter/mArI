using System.Text.Json.Serialization;

namespace mArI.Models;

public class ImageUrl
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}