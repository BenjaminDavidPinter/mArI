using System.Dynamic;
using System.Numerics;
using System.Text.Json.Serialization;

namespace mArI.Models;

public class Run
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }

    [JsonPropertyName("thread_id")]
    public string ThreadId { get; set; }

    [JsonPropertyName("assistant_id")]
    public string AssistantId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("required_action")]
    public RequiredAction? RequiredAction { get; set; }

    [JsonPropertyName("last_error")]
    public Error? LastError { get; set; }

    [JsonPropertyName("expires_at")]
    public long? ExpiresAt { get; set; }

    [JsonPropertyName("started_at")]
    public long? StartedAt { get; set; }

    [JsonPropertyName("cancelled_at")]
    public long? CancelledAt { get; set; }

    [JsonPropertyName("failed_at")]
    public long? FailedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public long? CompletedAt { get; set; }

    [JsonPropertyName("incomplete_details")]
    public IncompleteDetails? IncompleteDetails { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; }

    [JsonPropertyName("tools")]
    public List<Tool>? Tools { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("usage")]
    public UsageStatistics? Usage { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    [JsonPropertyName("max_prompt_tokens")]
    public int? MaxPromptTokens { get; set; }

    [JsonPropertyName("max_completion_tokens")]
    public int? MaxCompletionTokens { get; set; }

    [JsonPropertyName("truncation_strategy")]
    public TruncationStrategy? TruncationStrategy { get; set; }

    [JsonPropertyName("tool_choice")]
    public string ToolChoice { get; set; }

    [JsonPropertyName("parallel_tool_calls")]
    public bool? ParallelToolCalls { get; set; }

    [JsonPropertyName("response_format")]
    public object ResponseFormat { get; set; }
}