using System.Collections.Generic;

namespace CyberQuiz_Shared.DTOs
{
    // DTO för att skicka kontext om en fråga till AI-tjänsten
    public class QuestionContext
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> AnswerOptions { get; set; } = new List<string>();
    }
}
