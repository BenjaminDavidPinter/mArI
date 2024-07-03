using System.Text.Json.Serialization;

namespace mArI.Models;

public class Annotation
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("start_index")]
    public int? StartIndex { get; set; }

    [JsonPropertyName("end_index")]
    public int? EndIndex { get; set; }

    [JsonPropertyName("file_citation")]
    public FileCitation? FileCitation { get; set; }

    [JsonPropertyName("file_path")]
    public FilePath? FilePath { get; set; }
}