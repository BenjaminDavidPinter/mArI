using mArI.Model;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class ToolResource
{
    [JsonPropertyName("code_interpreter")]
    public CodeInterpreterToolResource CodeInterpreter { get; set; }
    
    [JsonPropertyName("file_search")]
    public FileSearchToolResource FileSearch { get; set; }
}