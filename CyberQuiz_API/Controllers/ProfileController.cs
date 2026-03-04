using CyberQuiz.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CyberQuiz_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserResultRepository _resultRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ProfileController(IUserResultRepository resultRepo, ICategoryRepository categoryRepo)
        {
            _resultRepo = resultRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            // Hämta alla subkategorier via DAL (utan DbContext i controller)
            var categories = await _categoryRepo.GetAllWithSubCategoriesAsync(ct);
            var allSubCategories = categories
                .SelectMany(c => c.SubCategories)
                .OrderBy(sc => sc.CategoryId)
                .ThenBy(sc => sc.Order)
                .Select(sc => new { sc.Id, sc.Name })
                .ToList();

            // Accuracy per subkategori (DAL räknar)
            var perSubCategory = new List<object>();
            foreach (var sc in allSubCategories)
            {
                var acc = await _resultRepo.GetAccuracyForUserInSubCategoryAsync(userId, sc.Id, ct);
                perSubCategory.Add(new
                {
                    subCategoryId = sc.Id,
                    subCategoryName = sc.Name,
                    accuracy = acc,
                    accuracyPercent = (int)Math.Round(acc * 100)
                });
            }

            // Totals (utan _db): räkna totals via userresults repo-lista
            // (Vi använder GetResultsForUserAndSubCategoryAsync per subkategori och summerar)
            // Enklast utan nya repo-metoder:
            var totalAnswers = 0;
            var totalCorrect = 0;

            foreach (var sc in allSubCategories)
            {
                var results = await _resultRepo.GetResultsForUserAndSubCategoryAsync(userId, sc.Id, ct);
                totalAnswers += results.Count;
                totalCorrect += results.Count(r => r.IsCorrect);
            }

            var totalAccuracy = totalAnswers == 0 ? 0 : (double)totalCorrect / totalAnswers;

            return Ok(new
            {
                totalAnswers,
                totalCorrect,
                totalAccuracy,
                totalAccuracyPercent = (int)Math.Round(totalAccuracy * 100),
                perSubCategory
            });
        }
    }
}
