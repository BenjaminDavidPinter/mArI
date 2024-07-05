using System.Text.Json.Serialization;

namespace mArI.Models;

public class RunStepList
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("data")]
    public List<RunStep> Steps { get; set; }
}