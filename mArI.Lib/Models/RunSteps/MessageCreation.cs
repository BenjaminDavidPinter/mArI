using System.Text.Json.Serialization;

namespace mArI.Models;

public class MessageCreation
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
}