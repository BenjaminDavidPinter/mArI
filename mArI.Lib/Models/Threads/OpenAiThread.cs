using System.Text.Json.Serialization;

namespace mArI.Models;

public class OpenAiThread
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("tool_resources")]
    public ToolResource ToolResources { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string> MetaData { get; set; }
}