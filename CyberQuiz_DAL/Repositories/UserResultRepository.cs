using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Repositories;

public class UserResultRepository : IUserResultRepository
{
    private readonly CyberQuizDbContext _db;

    private readonly object _context;

    public UserResultRepository(CyberQuizDbContext db) => _db = db;

    public async Task AddResultAsync(List<UserResult> result, CancellationToken ct = default)
    {
       await  _db.UserResults.AddRangeAsync(result, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<UserResult>> GetResultsForUserAndSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default)
        => _db.UserResults
              .AsNoTracking()
              .Where(r => r.UserId == userId && r.SubCategoryId == subCategoryId)
              .OrderByDescending(r => r.AnsweredAtUtc)
              .ToListAsync(ct);

    public async Task<double> GetAccuracyForUserInSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default)
    {
        var query = _db.UserResults.AsNoTracking()
            .Where(r => r.UserId == userId && r.SubCategoryId == subCategoryId);

        var total = await query.CountAsync(ct);
        if (total == 0) return 0;

        var correct = await query.CountAsync(r => r.IsCorrect, ct);
        return (double)correct / total;
    }
    public async Task<List<UserResult>> GetResultsByQuizAttemptIdAsync(Guid quizAttemptId,CancellationToken ct = default)
    {

        return await _db.UserResults
            .Include(r => r.SubCategory)
            .Where(r => r.QuizAttemptId == quizAttemptId)
            .ToListAsync(ct);
    }


}