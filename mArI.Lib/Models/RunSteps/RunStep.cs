namespace mArI.Models;

public class RunStep
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created_at { get; set; }
    public string assistant_id { get; set; }
    public string thread_id { get; set; }
    public string run_id { get; set; }
    public string @type { get; set; }
    public string status { get; set; }
    public StepDetail step_details { get; set; }
    public Error last_error { get; set; }
    public int expired_at { get; set; }
    public int cancelled_at { get; set; }
    public int failed_at { get; set; }
    public int completed_at { get; set; }
    public List<Dictionary<string, string>> metadata { get; set; }
    public UsageStatistics usage { get; set; }
}