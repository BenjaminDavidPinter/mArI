namespace mArI.Models;

public class UsageStatistics
{
    public int completion_tokens { get; set; }
    public int prompt_tokens { get; set; }
    public int total_tokens { get; set; }
}