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

            [HttpPost("chat")]
            public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken ct)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized(new { message = "Not logged in" });

                if (request == null || string.IsNullOrWhiteSpace(request.UserMessage))
                    return BadRequest(new { message = "Message cannot be empty" });

                var response = await _aiService.ChatAsync(request, ct);

                if (!response.Success)
                    return StatusCode(500, response);

                return Ok(response);
            }
        }
    }

