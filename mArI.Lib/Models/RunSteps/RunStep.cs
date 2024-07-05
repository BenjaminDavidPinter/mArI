using System.Text.Json.Serialization;

namespace mArI.Models;

public class RunStep
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }

    [JsonPropertyName("assistant_id")]
    public string? AssistantId { get; set; }

    [JsonPropertyName("thread_id")]
    public string? ThreadId { get; set; }

    [JsonPropertyName("run_id")]
    public string? RunId { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("step_details")]
    public StepDetail? StepDetails { get; set; }

    [JsonPropertyName("last_error")]
    public Error? LastError { get; set; }

    [JsonPropertyName("expired_at")]
    public long? expired_at { get; set; }

    [JsonPropertyName("cancelled_at")]
    public long? CancelledAt { get; set; }

    [JsonPropertyName("failed_at")]
    public long? FailedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public long? CompletedAt { get; set; }

    [JsonPropertyName("metadata")]
    public List<Dictionary<string, string>>? Metadata { get; set; }

    [JsonPropertyName("usage")]
    public UsageStatistics? Usage { get; set; }
}