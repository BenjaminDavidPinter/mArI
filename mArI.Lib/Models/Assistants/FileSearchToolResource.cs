using System.Text.Json.Serialization;

namespace mArI.Model;

public class FileSearchToolResource
{
    [JsonPropertyName("vector_store_ids")]
    public List<string>? VectorStoreIds { get; set; }
}
