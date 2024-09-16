using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class Tool
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("file_search")]
    public FileSearch? FileSearch { get; set; }
}