using System.Text.Json.Serialization;

namespace mArI.Models;

public class Assistant
{
    public Assistant(string model)
    {
        Model = model;
    }


    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; set; }

    [JsonPropertyName("tools")]
    public List<Tool>? Tools { get; set; }

    [JsonPropertyName("tool_resource")]
    public ToolResource? ToolResources { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public double TopP { get; set; }

    /*
    TODO: There is an issue with this property that I am unsure on how to solve; It 
    can either be a string or an object. This doesn't matter so much when we are 
    creating assistants. The issue arises when we call ListAssistants.

    Unlike other similar problems, like Message.cs, where we know the instances when
    the Content property is either a string, or a more complex object, ListAssistants 
    returns all assistants, whether they have the string version of this property, 
    or the object type.

    Therefore, when we initialize a backing store for the result of ListAssistants,
    we cannot specify (at least to my current knowledge), that it can be 'either or'.
    There's no instance of 'union' in C#, nor do I want to use Dynamic.
    */
    // [JsonPropertyName("response_format")]
    // public string ResponseFormat { get; set; }
}