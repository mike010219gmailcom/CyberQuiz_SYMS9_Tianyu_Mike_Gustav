using CyberQuiz.DAL.Models;
using CyberQuiz.DAL.Repositories;
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
        private readonly IQuizRepository _quizRepo;
        private readonly IUserResultRepository _resultRepo;

        public QuizController(IQuizRepository quizRepo, IUserResultRepository resultRepo)
        {
            _quizRepo = quizRepo;
            _resultRepo = resultRepo;
        }


        [HttpGet("start/{subCategoryId:int}")]
        public async Task<IActionResult> Start(int subCategoryId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var sub = await _quizRepo.GetSubCategoryWithQuestionsAsync(subCategoryId, ct);
            if (sub is null)
                return NotFound(new { message = "SubCategory not found" });

            // Mapping i controller
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

            return Ok(new
            {
                isCorrect,
                accuracy,
                accuracyPercent = (int)Math.Round(accuracy * 100)
            });
        }
    }
}
