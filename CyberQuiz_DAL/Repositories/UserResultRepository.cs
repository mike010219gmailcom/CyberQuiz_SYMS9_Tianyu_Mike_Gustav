using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Repositories;

// Repository-klass som hanterar databasoperationer för användarresultat
public class UserResultRepository : IUserResultRepository
{
    private readonly CyberQuizDbContext _db;

    private readonly object _context;

    // Konstruktor – tar emot databaskontexten via dependency injection
    public UserResultRepository(CyberQuizDbContext db) => _db = db;

    // Sparar en lista med användarresultat till databasen
    public async Task AddResultAsync(List<UserResult> result, CancellationToken ct = default)
    {
       await  _db.UserResults.AddRangeAsync(result, ct);
        await _db.SaveChangesAsync(ct);
    }

    // Hämtar alla resultat för en specifik användare i en specifik underkategori
    public Task<List<UserResult>> GetResultsForUserAndSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default)
        => _db.UserResults
              .AsNoTracking()
              .Where(r => r.UserId == userId && r.SubCategoryId == subCategoryId)
              .OrderByDescending(r => r.AnsweredAtUtc)
              .ToListAsync(ct);

    // Beräknar träffsäkerheten (andel rätta svar) för en användare i en underkategori
    public async Task<double> GetAccuracyForUserInSubCategoryAsync(
        string userId,
        int subCategoryId,
        CancellationToken ct = default)
    {
        // Basquery – filtrerar på användare och underkategori
        var query = _db.UserResults.AsNoTracking()
            .Where(r => r.UserId == userId && r.SubCategoryId == subCategoryId);

        var total = await query.CountAsync(ct);
        // Returnerar 0 om användaren inte har svarat på något
        if (total == 0) return 0;

        var correct = await query.CountAsync(r => r.IsCorrect, ct);
        return (double)correct / total;
    }

    // Hämtar alla resultat kopplade till ett specifikt quiz-försök via dess unika ID
    public async Task<List<UserResult>> GetResultsByQuizAttemptIdAsync(Guid quizAttemptId,CancellationToken ct = default)
    {

        return await _db.UserResults
            .Include(r => r.SubCategory)
            .Where(r => r.QuizAttemptId == quizAttemptId)
            .ToListAsync(ct);
    }


}