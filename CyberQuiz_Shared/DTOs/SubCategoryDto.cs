using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.DTOs
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public int QuestionCount { get; set; }
        public bool IsLocked { get; set; }
    }
}
