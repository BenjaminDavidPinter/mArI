using System.Text.Json.Serialization;

namespace mArI.Models;

public class MessageText {
    [JsonPropertyName("type")]
    public string Type {get;set;}

    [JsonPropertyName("text")]
    public string Text {get;set;}
}