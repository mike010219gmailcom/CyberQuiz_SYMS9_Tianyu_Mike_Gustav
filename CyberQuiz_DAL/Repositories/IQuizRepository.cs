using CyberQuiz.DAL.Models;

namespace CyberQuiz.DAL.Repositories;

public interface IQuizRepository
{
    Task<SubCategory?> GetSubCategoryWithQuestionsAsync(int subCategoryId, CancellationToken ct = default);
    Task<Question?> GetQuestionWithOptionsAsync(int questionId, CancellationToken ct = default);
    Task<int> GetQuestionCountForSubCategoryAsync(int subCategoryId, CancellationToken ct = default);
}