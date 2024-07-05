using System.Text.Json.Serialization;

namespace mArI.Models;

public class StepDetail
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("message_creation")]
    public MessageCreation MessageCreation { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<Tool> ToolCalls { get; set; }
}