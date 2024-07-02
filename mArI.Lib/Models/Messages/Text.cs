using System.Text.Json.Serialization;

namespace mArI.Models;

public class Text
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("annotations")]
    public List<Annotation>? Annotations { get; set; }
}