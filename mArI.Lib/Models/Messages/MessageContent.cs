using System.Text.Json.Serialization;

namespace mArI.Models;

public class MessageContent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("image_file")]
    public ImageFile? ImageFile { get; set; }

    [JsonPropertyName("image_url")]
    public ImageUrl? ImageUrl { get; set; }

    [JsonPropertyName("text")]
    public Text? Text { get; set; }
}