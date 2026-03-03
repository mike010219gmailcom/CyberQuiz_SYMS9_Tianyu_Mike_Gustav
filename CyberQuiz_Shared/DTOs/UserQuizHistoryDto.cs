using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class UserQuizHistoryDto
    {
        public string UserId { get; set; }
        public List<QuizSummaryDto> UserQuizHistory { get; set; } = new();

    }
}
