namespace mArI.Models;

public class Annotation
{
    public string type { get; set; }
    public string text { get; set; }
    public int start_index { get; set; }
    public int end_index { get; set; }
    public FileCitation file_citation { get; set; }
    public FilePath file_path { get; set; }
}