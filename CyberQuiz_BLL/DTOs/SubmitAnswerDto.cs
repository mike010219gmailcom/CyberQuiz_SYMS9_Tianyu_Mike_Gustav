using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerOptionId { get; set; }
    }
}
