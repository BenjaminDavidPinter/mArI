using System.Text.Json;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class Assistant
{
    public Assistant(string model)
    {
        Model = model;
    }

    #region OpenAI Type Definition
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; set; }

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

    [JsonPropertyName("response_format")]
    public object? ResponseFormat { get; set; }

    #endregion

    [JsonIgnore]
    public string ResponseFormatAsString
    {
        get
        {
            return (string)ResponseFormat;
        }
    }

    [JsonIgnore]
    public ResponseFormat ResponseFormatAsObject
    {
        get
        {
            return JsonSerializer.Deserialize<ResponseFormat>(ResponseFormatAsString ?? "{}");
        }
    }


}