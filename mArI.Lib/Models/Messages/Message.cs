using System.Dynamic;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class Message
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    [JsonPropertyName("thread_id")]
    public string? ThreadId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("incomplete_details")]
    public IncompleteDetails? IncompleteDetails { get; set; }

    [JsonPropertyName("completed_at")]
    public int? CompletedAt { get; set; }

    [JsonPropertyName("incomplete_at")]
    public int? IncompleteAt { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public List<MessageContent>? Content { get; set; }

    [JsonPropertyName("assistant_id")]   
    public string? AssistantId { get; set; }

    [JsonPropertyName("run_id")]
    public string? RunId { get; set; }

    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }
}