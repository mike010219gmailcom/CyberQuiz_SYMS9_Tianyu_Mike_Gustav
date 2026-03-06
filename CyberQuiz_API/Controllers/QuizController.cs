using CyberQuiz.DAL.Models;
using CyberQuiz.DAL.Repositories;
using CyberQuiz_BLL.DTOs;
using CyberQuiz_BLL.Interfaces;
using CyberQuiz_Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CyberQuiz_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IUserProgressService _userProgressService;
        //private readonly ICategoryService _categoryService;

        public QuizController(IQuizService quizService, IUserProgressService userProgressService)
        {
            _quizService = quizService;
            _userProgressService = userProgressService;
        }

        // Get /quiz/questions/1
        [HttpGet("start/{subCategoryId:int}")]
        public async Task<IActionResult> Start(int subCategoryId, CancellationToken ct)
        {
            // Get questions
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var questions = await _quizService.GetQuestionsAsync(subCategoryId, userId, ct);

            return Ok(questions);
        }


        // POST: api/quiz/answer
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto dto, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var summary = await _quizService.SubmitQuizAsync(userId, dto, ct);

            return Ok(summary);
        }

        // GET: api/quiz/summary
        [HttpGet("summary/{quizAttemptId:guid}")]
        public async Task<IActionResult> Summary(string userId, Guid quizAttemptId, CancellationToken ct)
        {
            var summary = await _quizService.GetQuizSummaryAsync(userId, quizAttemptId, ct);

            if (summary == null)
                return NotFound();

            return Ok(summary);
        }
    }
}
