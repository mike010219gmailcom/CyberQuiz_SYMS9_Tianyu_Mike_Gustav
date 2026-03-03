using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class QuizSummaryDto
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
        public string CategoryName { get; set;}
        public string SubCategoryName { get; set; }

        public DateTimeOffset CompletedAtUtc { get; set; }
    }
}
