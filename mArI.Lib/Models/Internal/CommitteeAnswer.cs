namespace mArI.Models;

public class CommitteeAnswer<T>
{
    public Assistant<T> AssistantInfo { get; set; }
    public OpenAiThread ThreadInfo { get; set; }
    public Run RunInfo { get; set; }
    public Message<List<MessageContent>> Answer { get; set; }

    public string GetAnswerAsText()
    {
        return Answer.Content.First().Text.Value;
    }
}