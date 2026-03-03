using CyberQuiz_BLL.DTOs;
using CyberQuiz_Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface IQuizService
    {
        //Get
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<SubCategoryDto>> GetSubCategoriesAsync(int categoryId);
        Task<List<QuestionDto>> GetQuestionsAsync(int subCategoryId);
        Task<QuizSummaryDto> SubmitQuizAysnc(string userId, SubmitQuizDto dto);
        //Task<QuizSummaryDto> GetQuizSummaryAsync(string userId, int subCategoryId); 
        Task<UserQuizHistoryDto> GetUserQuizHistoryAsync(string userId);
    }
}
