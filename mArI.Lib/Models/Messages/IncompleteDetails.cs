using System.Text.Json.Serialization;

namespace mArI.Models;

public class IncompleteDetails
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}