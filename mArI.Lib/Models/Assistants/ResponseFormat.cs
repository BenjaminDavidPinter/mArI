using System.Text.Json.Serialization;

namespace mArI.Models;

public class ResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}