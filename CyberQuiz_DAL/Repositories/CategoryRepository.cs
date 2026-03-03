using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CyberQuizDbContext _db;

    public CategoryRepository(CyberQuizDbContext db) => _db = db;

    public Task<List<Category>> GetAllWithSubCategoriesAsync(CancellationToken ct = default)
        => _db.Categories
              .AsNoTracking()
              .Include(c => c.SubCategories.OrderBy(sc => sc.Order))
              .OrderBy(c => c.Name)
              .ToListAsync(ct);

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}