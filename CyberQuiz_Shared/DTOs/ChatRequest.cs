using System.Collections.Generic;

namespace CyberQuiz_Shared.DTOs
{
    // DTO för att skicka chattförfrågningar till AI-tjänsten
    public class ChatRequest
    {
        public string UserMessage { get; set; } = string.Empty;
        public List<ChatMessage> ConversationHistory { get; set; } = new List<ChatMessage>();
        public QuestionContext? CurrentQuestion { get; set; }
    }
}
