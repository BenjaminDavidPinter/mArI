using System.Text.Json.Serialization;

namespace mArI.Models;

public class Assistant
{
    public Assistant(string model)
    {
        Model = model;
    }


    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; }

    [JsonPropertyName("tools")]
    public List<Tool>? Tools { get; set; }

    [JsonPropertyName("tool_resource")]
    public ToolResource? ToolResources { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    //TODO: This needs to act like a union, it can 
    //      be either a string, or an object
    //      I know I've got code around here
    //      somewhere for that
    [JsonPropertyName("response_format")]
    public string ResponseFormat { get; set; }
}