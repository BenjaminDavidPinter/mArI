using System.Text.Json.Serialization;

namespace mArI.Models;

public class FilePath
{
    [JsonPropertyName("file_id")]
    public string? FileId { get; set; }
}