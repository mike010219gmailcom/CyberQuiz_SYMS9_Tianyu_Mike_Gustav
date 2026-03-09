using CyberQuiz_BLL.Interfaces;
using CyberQuiz_BLL.Models;
using CyberQuiz_Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class AiService: IAiService
    {
        private readonly IUserProgressService _userProgressService;
        private readonly HttpClient _http;

        public AiService(
            IUserProgressService userProgressService, HttpClient http)
        {
            _userProgressService = userProgressService;
            _http = http;
        }

        public async Task<AiCoachFeedbackDto> GenerateCoachFeedbackAsync(string userId, CancellationToken ct)
        {
            // Get userProfile data
            var profile = await _userProgressService.GetFullUserProfileAsync(userId, ct);

            // Convert progress into readable summary
            var summary = string.Join("\n",
                profile.PerSubCategory.Select(sc =>
                    $"{sc.SubCategoryName}: {sc.AccuracyPercent}% correct"));

            // Create prompt
            var prompt = $"""
            Du är en AI-inlärningscoach.

            Analysera prestationsdata från detta quiz.

            Ange:
            - styrkor
            - svagheter
            - rekommendationer om vad du ska studera härnäst.

            Data:Du är en AI-inlärningscoach.

            Analysera prestationsdata från detta quiz.

            Ange:
            - styrkor
            - svagheter
            - rekommendationer om vad du ska studera härnäst.

            Data:

            {summary}
            """;

            // call AI model
            var request = new
            {
                model = "phi3",
                prompt = prompt,
                stream = false
            };

            var response = await _http.PostAsJsonAsync("/api/generate", request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new AiCoachFeedbackDto
                {
                    Feedback = "AI-coachen är för närvarande inte tillgänglig."
                };
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: ct);

            return new AiCoachFeedbackDto
            {
                Feedback = result?.response ?? "Ingen AI-feedback tillgänglig."
            };
        }
    }
}
