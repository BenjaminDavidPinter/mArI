using System.Text.Json.Serialization;

namespace mArI.Models;


public class VectorStoreFileError
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}

