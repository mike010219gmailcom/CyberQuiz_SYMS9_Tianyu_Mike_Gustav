using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_Shared.DTOs
{
    public class SubmitQuizDto // list of answer
    {
        public int SubCategoryId { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }
}
