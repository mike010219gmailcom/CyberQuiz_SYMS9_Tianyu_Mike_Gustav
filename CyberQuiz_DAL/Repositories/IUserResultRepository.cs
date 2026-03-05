using CyberQuiz.DAL.Models;

namespace CyberQuiz.DAL.Repositories;

public interface IUserResultRepository
{
    Task AddResultAsync(List<UserResult> result, CancellationToken ct = default);

    Task<List<UserResult>> GetResultsForUserAndSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default);

    Task<double> GetAccuracyForUserInSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default);

    Task<List<UserResult>> GetResultsByQuizAttemptIdAsync(
        Guid quizAttemptId,
        CancellationToken ct = default);


}