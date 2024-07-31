using System.Text.Json.Serialization;

namespace mArI.Models;

public class ChunkingStrategy
{
    [JsonPropertyName("type")]
    public string? @Type { get; set; }

    [JsonPropertyName("static")]
    public StaticChunkStrategyOptions Static { get; set; }
}
