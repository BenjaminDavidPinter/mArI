using System.Text.Json.Serialization;

namespace mArI.Models;

public class TruncationStrategy
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("last_messages")]
    public int? LastMessages { get; set; }
}