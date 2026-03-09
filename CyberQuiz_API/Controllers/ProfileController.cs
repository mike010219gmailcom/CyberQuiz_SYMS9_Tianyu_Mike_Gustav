
using CyberQuiz_BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(
            IUserProgressService userProgressService,
            UserManager<IdentityUser> userManager)
        {
            _userProgressService = userProgressService;
            _userManager = userManager;
        }

        // NEW: Get full user profile (all subcategories + total stats)
        [HttpGet]
        public async Task<IActionResult> GetFullProfile(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var result = await _userProgressService.GetFullUserProfileAsync(userId, ct);

            return Ok(result);
        }

        // Get quiz history for specific subcategory
        [HttpGet("{subCategoryId:int}")]
        public async Task<IActionResult> GetUserProgress(int subCategoryId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var result = await _userProgressService
            .GetQuizHistoryForSubCategoryAsync(userId, subCategoryId, ct);

            return Ok(result);
        }

        // Change email
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Validate new email
            if (string.IsNullOrWhiteSpace(request.NewEmail))
                return BadRequest(new { message = "E-post får inte vara tom" });

            if (request.NewEmail != request.ConfirmEmail)
                return BadRequest(new { message = "E-postadresserna matchar inte" });

            // Check if email is already taken
            var existingUser = await _userManager.FindByEmailAsync(request.NewEmail);
            if (existingUser != null && existingUser.Id != userId)
                return BadRequest(new { message = "E-postadressen används redan" });

            // Change email
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, token);

            if (result.Succeeded)
            {
                // Also update username to match new email
                await _userManager.SetUserNameAsync(user, request.NewEmail);
                return Ok(new { message = "E-post uppdaterad!" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = $"Kunde inte uppdatera e-post: {errors}" });
        }

        // Change password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Not logged in" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Validate passwords
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return BadRequest(new { message = "Nuvarande lösenord krävs" });

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Nytt lösenord får inte vara tomt" });

            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new { message = "Lösenorden matchar inte" });

            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Lösenordet måste vara minst 6 tecken långt" });

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Lösenord uppdaterat!" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = $"Kunde inte uppdatera lösenord: {errors}" });
        }
    }

    // Request models
    public class ChangeEmailRequest
    {
        public string NewEmail { get; set; } = "";
        public string ConfirmEmail { get; set; } = "";
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
