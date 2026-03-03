using CyberQuiz.DAL.Models;

namespace CyberQuiz.DAL.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllWithSubCategoriesAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
}
