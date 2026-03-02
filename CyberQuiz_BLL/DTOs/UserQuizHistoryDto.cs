using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class UserQuizHistoryDto
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnwers { get; set; }
        public double ScorePercentage { get; set; }
        public DateTime CompleteTime { get; set; }
        public string CategoryName { get; set; }

    }
}
