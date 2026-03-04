using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CyberQuiz_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly CyberQuizDbContext _db;
        private readonly IUserResultRepository _resultRepo;

        public ProfileController(CyberQuizDbContext db, IUserResultRepository resultRepo)
        {
            _db = db;
            _resultRepo = resultRepo;
        }

        // GET: api/profile
        // Översikt: accuracy per subkategori + totals
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var subCategories = await _db.SubCategories
                .AsNoTracking()
                .OrderBy(s => s.CategoryId).ThenBy(s => s.Order)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync(ct);

            var perSubCategory = new List<object>();
            foreach (var sc in subCategories)
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

            var totalAnswers = await _db.UserResults.AsNoTracking()
                .CountAsync(r => r.UserId == userId, ct);

            var totalCorrect = await _db.UserResults.AsNoTracking()
                .CountAsync(r => r.UserId == userId && r.IsCorrect, ct);

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
