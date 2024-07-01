namespace mArI.Models;

public class StepDetail 
{
    public string @type {get;set;}
    public MessageCreation message_creation {get;set;}
    public List<Tool> tool_calls {get;set;}
}