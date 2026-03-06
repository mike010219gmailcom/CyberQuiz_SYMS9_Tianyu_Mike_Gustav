
using CyberQuiz_BLL.Interfaces;
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
        private readonly IUserProgressService _userProgressService;

        public ProfileController(IUserProgressService userProgressService)
        {
            _userProgressService = userProgressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProgress(int subCategoryId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var result = await _userProgressService
            .GetQuizHistoryForSubCategoryAsync(userId, subCategoryId, ct);

            return Ok(result);
        }
    }
}
