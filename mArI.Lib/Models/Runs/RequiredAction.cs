using System.Text.Json.Serialization;

namespace mArI.Models;

public class RequiredAction
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("submit_tool_outputs")]
    public SubmitToolOutputs? SubmitToolOutputs { get; set; }
}