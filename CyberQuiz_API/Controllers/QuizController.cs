using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
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
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepo;
        private readonly IUserResultRepository _resultRepo;
        private readonly CyberQuizDbContext _db;

        public QuizController(
            IQuizRepository quizRepo,
            IUserResultRepository resultRepo,
            CyberQuizDbContext db)
        {
            _quizRepo = quizRepo;
            _resultRepo = resultRepo;
            _db = db;
        }

        // GET: api/quiz/start/5
        // Returnerar frågor + svarsalternativ (utan IsCorrect)
        [HttpGet("start/{subCategoryId:int}")]
        public async Task<IActionResult> Start(int subCategoryId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var sub = await _quizRepo.GetSubCategoryWithQuestionsAsync(subCategoryId, ct);
            if (sub is null)
                return NotFound(new { message = "SubCategory not found" });

            // Skicka aldrig IsCorrect till UI
            var response = new
            {
                subCategory = new { id = sub.Id, name = sub.Name },
                questions = sub.Questions
                    .OrderBy(q => q.Id)
                    .Select(q => new
                    {
                        id = q.Id,
                        text = q.Text,
                        answerOptions = q.AnswerOptions
                            .OrderBy(a => a.Id)
                            .Select(a => new { id = a.Id, text = a.Text })
                    })
            };

            return Ok(response);
        }

        public sealed class SubmitAnswerRequest
        {
            public int SubCategoryId { get; set; }
            public int QuestionId { get; set; }
            public int SelectedAnswerOptionId { get; set; }
        }

        // POST: api/quiz/answer
        // Sparar resultat i DB och returnerar feedback + progression
        [HttpPost("answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest req, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (req.SubCategoryId <= 0 || req.QuestionId <= 0 || req.SelectedAnswerOptionId <= 0)
                return BadRequest(new { message = "Invalid request" });

            var question = await _quizRepo.GetQuestionWithOptionsAsync(req.QuestionId, ct);
            if (question is null)
                return NotFound(new { message = "Question not found" });

            if (question.SubCategoryId != req.SubCategoryId)
                return BadRequest(new { message = "Question does not belong to subcategory" });

            var selected = question.AnswerOptions.FirstOrDefault(a => a.Id == req.SelectedAnswerOptionId);
            if (selected is null)
                return BadRequest(new { message = "Answer option not found" });

            var isCorrect = selected.IsCorrect;

            // Krav: spara varje svar
            var result = new UserResult
            {
                UserId = userId,
                SubCategoryId = req.SubCategoryId,
                QuestionId = req.QuestionId,
                SelectedAnswerOptionId = req.SelectedAnswerOptionId,
                IsCorrect = isCorrect,
                AnsweredAtUtc = DateTimeOffset.UtcNow
            };

            await _resultRepo.AddResultAsync(result, ct);

            
            var accuracy = await _resultRepo.GetAccuracyForUserInSubCategoryAsync(userId, req.SubCategoryId, ct);

            // Hitta nästa subkategori inom samma kategori baserat på Order
            var currentSub = await _db.SubCategories.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == req.SubCategoryId, ct);

            int? nextSubCategoryId = null;
            var unlockedNext = false;

            if (currentSub is not null)
            {
                nextSubCategoryId = await _db.SubCategories.AsNoTracking()
                    .Where(s => s.CategoryId == currentSub.CategoryId && s.Order > currentSub.Order)
                    .OrderBy(s => s.Order)
                    .Select(s => (int?)s.Id)
                    .FirstOrDefaultAsync(ct);

                unlockedNext = accuracy >= 0.8 && nextSubCategoryId is not null;
            }

            return Ok(new
            {
                isCorrect,
                accuracy,
                accuracyPercent = (int)Math.Round(accuracy * 100),
                unlockedNext,
                nextSubCategoryId
            });
        }
    }
}
