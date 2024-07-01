using System.Dynamic;
using System.Numerics;

namespace mArI.Models;

public class Run
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created_at { get; set; }
    public string thread_id { get; set; }
    public string assistant_id { get; set; }
    public string status { get; set; }
    public RequiredAction required_action { get; set; }
    public Error last_error { get; set; }
    public int expires_at { get; set; }
    public int started_at { get; set; }
    public int cancelled_at { get; set; }
    public int failed_at { get; set; }
    public int completed_at { get; set; }
    public IncompleteDetails incomplete_details { get; set; }
    public string model { get; set; }
    public string instructions { get; set; }
    public List<Tool> tools { get; set; }
    public Dictionary<string, string> metadata { get; set; }
    public UsageStatistics usage { get; set; }
    public double temperature { get; set; }
    public double top_p { get; set; }
    public int max_prompt_tokens { get; set; }
    public int max_completion_tokens { get; set; }
    public TruncationStrategy truncation_strategy { get; set; }
    //NOTE: Object or String?
    public string tool_choice { get; set; }
    public bool parallel_tool_calls { get; set; }
    //NOTE: Object or String?
    public string response_format { get; set; }
}