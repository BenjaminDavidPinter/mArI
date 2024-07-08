using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class ImageFile
{
    [JsonPropertyName("image_file")]
    public ImageFileDetail? FileDetails { get; set; }
    
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("type")]
    public string? Type {get;set;}
}