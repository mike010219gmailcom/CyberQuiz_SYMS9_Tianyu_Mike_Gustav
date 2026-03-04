using CyberQuiz.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CyberQuiz_API.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IQuizRepository _quizRepo;
        private readonly IUserResultRepository _resultRepo;

        public CategoriesController(
            ICategoryRepository categoryRepo,
            IQuizRepository quizRepo,
            IUserResultRepository resultRepo)
        {
            _categoryRepo = categoryRepo;
            _quizRepo = quizRepo;
            _resultRepo = resultRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var categories = await _categoryRepo.GetAllWithSubCategoriesAsync(ct);

            var response = new List<object>();

            foreach (var cat in categories)
            {
                var subs = cat.SubCategories.OrderBy(s => s.Order).ToList();
                var subDtos = new List<object>();

                for (int i = 0; i < subs.Count; i++)
                {
                    var sc = subs[i];

                    var questionCount = await _quizRepo.GetQuestionCountForSubCategoryAsync(sc.Id, ct);

                    // Första subkategorin i varje kategori är upplåst.
                    // Övriga låses upp om user har >=80% i föregående subkategori.
                    var isLocked = false;
                    if (i > 0)
                    {
                        var prev = subs[i - 1];
                        var prevAcc = await _resultRepo.GetAccuracyForUserInSubCategoryAsync(userId, prev.Id, ct);
                        isLocked = prevAcc < 0.8;
                    }

                    subDtos.Add(new
                    {
                        id = sc.Id,
                        name = sc.Name,
                        order = sc.Order,
                        questionCount,
                        isLocked
                    });
                }

                response.Add(new
                {
                    id = cat.Id,
                    name = cat.Name,
                    subCategories = subDtos
                });
            }

            return Ok(response);
        }
    }
}