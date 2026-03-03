using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Repositories;

public class UserResultRepository : IUserResultRepository
{
    private readonly CyberQuizDbContext _db;

    public UserResultRepository(CyberQuizDbContext db) => _db = db;

    public async Task AddResultAsync(UserResult result, CancellationToken ct = default)
    {
        _db.UserResults.Add(result);
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
}