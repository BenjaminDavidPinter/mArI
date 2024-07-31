using System;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class VectorStoreExpirationInformation
{
    [JsonPropertyName("anchor")]
    public string Anchor { get; set; }
    [JsonPropertyName("days")]
    public int Days { get; set; }
}
