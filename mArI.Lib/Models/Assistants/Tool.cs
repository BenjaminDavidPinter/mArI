using System.Reflection.Metadata.Ecma335;

namespace mArI.Models;

public class Tool
{
    public string type { get; set; }
    public FileSearch file_search { get; set; }
    public Function function { get; set; }
}