using System.Dynamic;
using System.Net.Mail;

namespace mArI.Models;

public class Message
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created_at { get; set; }
    public string thread_id { get; set; }
    public string status { get; set; }
    public IncompleteDetails incomplete_details { get; set; }
    public int completed_at { get; set; }
    public int incomplete_at { get; set; }
    public string role { get; set; }
    public List<MessageContent> content { get; set; }
    public string assistant_id { get; set; }
    public string run_id { get; set; }
    public List<Attachment> attachments { get; set; }
    public Dictionary<string, string> metadata { get; set; }
}