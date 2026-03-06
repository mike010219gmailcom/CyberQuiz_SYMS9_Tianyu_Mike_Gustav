using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class QuizSummaryDto
    {
        public Guid QuizAttemptId { get; set; } // update in model
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
        //public string CategoryName { get; set;}
        public int SubCategoryId {  get; set; }
        public string SubCategoryName { get; set; } = "";

        public DateTimeOffset CompletedAtUtc { get; set; }

        public bool Passed => ScorePercentage >= 80;
        public bool UnlockNext => ScorePercentage >= 80;
    }
}
