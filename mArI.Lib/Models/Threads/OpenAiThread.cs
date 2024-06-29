namespace mArI.Models;

public class OpenAiThread
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created_at { get; set; }
    public List<ToolResource> tool_resources { get; set; }
    public Dictionary<string, string> metadata { get; set; }
}