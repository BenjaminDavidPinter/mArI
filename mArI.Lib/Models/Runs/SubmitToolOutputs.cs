using System.Text.Json.Serialization;

namespace mArI.Models;

public class SubmitToolOutputs
{
    [JsonPropertyName("tool_calls")]
    public List<Tool>? ToolCalls { get; set; }
}