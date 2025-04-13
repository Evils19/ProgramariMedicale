namespace MedProgramari.Common.DTO;

public class ChatMessage
{
    public string Content { get; set; }
    public bool IsFromUser { get; set; }
    public DateTime Timestamp { get; set; }
}