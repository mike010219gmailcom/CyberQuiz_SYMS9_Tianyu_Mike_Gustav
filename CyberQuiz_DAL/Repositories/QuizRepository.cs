using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly CyberQuizDbContext _db;

    public QuizRepository(CyberQuizDbContext db) => _db = db;

    public Task<SubCategory?> GetSubCategoryWithQuestionsAsync(int subCategoryId, CancellationToken ct = default)
        => _db.SubCategories
              .AsNoTracking()
              .Include(sc => sc.Questions)
                .ThenInclude(q => q.AnswerOptions.OrderBy(a => a.Id))
              .FirstOrDefaultAsync(sc => sc.Id == subCategoryId, ct);

    public Task<Question?> GetQuestionWithOptionsAsync(int questionId, CancellationToken ct = default)
        => _db.Questions
              .AsNoTracking()
              .Include(q => q.AnswerOptions.OrderBy(a => a.Id))
              .FirstOrDefaultAsync(q => q.Id == questionId, ct);

    public Task<int> GetQuestionCountForSubCategoryAsync(int subCategoryId, CancellationToken ct = default)
        => _db.Questions.CountAsync(q => q.SubCategoryId == subCategoryId, ct);
}