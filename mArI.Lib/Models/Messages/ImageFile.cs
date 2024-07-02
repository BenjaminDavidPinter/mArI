using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class ImageFile
{
    [JsonPropertyName("file_id")]
    public string? FileId { get; set; }
    
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}