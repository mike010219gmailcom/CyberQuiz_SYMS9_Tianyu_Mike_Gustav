using CyberQuiz_BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CyberQuiz_API.Controllers

{
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class AiController : ControllerBase
        {
            private readonly IAiService _aiService;

            public AiController(IAiService aiService)
            {
                _aiService = aiService;
            }

            [HttpGet("coach")]
            public async Task<IActionResult> GetCoachFeedback(CancellationToken ct)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized(new { message = "Not logged in" });

                var feedback = await _aiService.GenerateCoachFeedbackAsync(userId, ct);
                return Ok(feedback);
            }
        }
    }

