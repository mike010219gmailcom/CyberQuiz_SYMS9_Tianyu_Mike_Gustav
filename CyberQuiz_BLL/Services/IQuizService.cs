using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public interface IQuizService
    {
        //Get
        Task<List<CategoryDto>> GetCategorysync();
        Task<List<SubCategoryDto>> GetSubCategoryAsync(int categoryId);
        Task<List<QuestionDto>> GetQuestionAsync(int subCategoryId);
        Task<bool> SubmitAnswerAysnc(string userId, SubmitAnswerDto dto);
        Task<QuizSummaryDto> GetQuizSummaryAsync(string userId, int subCategoryId);
        Task<UserQuizHistoryDto> GetUserQuizHistoryAsync(string userId);
    }
}
