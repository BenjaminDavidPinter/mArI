using System.Text.Json.Serialization;

namespace mArI.Models;

public class VectorStoreFile
{
    [JsonPropertyName("id")]
    public string? Id { get; set; } 

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("usage_bytes")]
    public int? UsageBytes { get; set; }

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    [JsonPropertyName("vector_store_id")]
    public string? VectorStoreId { get; set; }

    [JsonPropertyName("status")]
    public string? status { get; set; }

    [JsonPropertyName("last_error")]
    public VectorStoreFileError LastError { get; set; }

    [JsonPropertyName("chunking_strategy")]
    public ChunkingStrategy? ChunkingStrategy { get; set; }
}
