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
        //Task<List<CategoryDto>> GetCategoriesAsync();
        //Task<List<SubCategoryDto>> GetSubCategoriesAsync(int categoryId);
        Task<List<QuestionDto>> GetQuestionsAsync(int subCategoryId, string userId, CancellationToken ct = default);
        Task<QuizSummaryDto> SubmitQuizAysnc(string userId, SubmitQuizDto dto, CancellationToken ct = default);
        Task<QuizSummaryDto> GetQuizSummaryAsync(string userid, int subcategoryid);
    }
}
