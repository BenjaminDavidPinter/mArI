namespace mArI.Models;

public class Assistant
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created_at { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string model { get; set; }
    public string instructions { get; set; }
    public List<Tool> tools { get; set; }
    public List<ToolResource> tool_resources { get; set; }
    public Dictionary<string, string> metadata { get; set; }
    public double temperature { get; set; }
    public double top_p { get; set; }
    public string response_format { get; set; }
}