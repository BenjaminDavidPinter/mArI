using System.Text.Json.Serialization;

namespace mArI.Model;

public class CodeInterpreterToolResource
{
    [JsonPropertyName("file_ids")]
    List<string>? FileIds { get; set; }
}
