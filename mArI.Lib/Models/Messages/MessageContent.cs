namespace mArI.Models;

public class MessageContent
{
    public string type { get; set; }
    public ImageFile image_file { get; set; }
    public ImageUrl image_url { get; set; }
    public Text text { get; set; }
}