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

        public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken ct)
        {
            try
            {
                // konversations prompt som inkluderar regler, aktuell fråga, historik och användarens senaste meddelande
                var conversationPrompt = BuildConversationPrompt(request);

                // Temperatur och sampling-inställningar för att styra AI:ns svar
                var ollamaRequest = new
                {
                    model = "phi3",
                    prompt = conversationPrompt,
                    stream = false,
                    options = new
                    {
                        temperature = 0.7,  // Lägre, den hittar på ord mindre ofta
                        top_p = 0.9,        // Högre, den kan välja mer ovanliga ord
                        top_k = 40          // Högre, den kan välja från fler ord 
                    }
                };

                var response = await _http.PostAsJsonAsync("/api/generate", ollamaRequest, ct);

                if (!response.IsSuccessStatusCode)
                {
                    return new ChatResponse
                    {
                        Success = false,
                        ErrorMessage = "AI-chatten är för närvarande inte tillgänglig."
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: ct);

                return new ChatResponse
                {
                    Message = result?.response ?? "Ingen respons från AI.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage = $"Ett fel uppstod: {ex.Message}"
                };
            }
        }

        private string BuildConversationPrompt(ChatRequest request)
        {
            var promptBuilder = new StringBuilder();

            // Instruktioner och regler för AI:n
            promptBuilder.AppendLine("Du är CyberChat, en hjälpsam IT-säkerhetsassistent.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("REGLER:");
            promptBuilder.AppendLine("- Användaren gör ett quiz. Du får ALDRIG säga vilket svar som är rätt.");
            promptBuilder.AppendLine("- Ge pedagogiska förklaringar och ledtrådar utan att avslöja svaret.");
            promptBuilder.AppendLine("- Svara kort och tydligt (2-4 meningar).");
            promptBuilder.AppendLine("- Använd vanliga svenska ord för IT-säkerhet.");
            promptBuilder.AppendLine();

            // Lägg till aktuell fråga och alternativ
            if (request.CurrentQuestion != null)
            {
                promptBuilder.AppendLine($"Fråga: {request.CurrentQuestion.QuestionText}");
                if (request.CurrentQuestion.AnswerOptions?.Any() == true)
                {
                    promptBuilder.AppendLine("Alternativ:");
                    foreach (var option in request.CurrentQuestion.AnswerOptions)
                    {
                        promptBuilder.AppendLine($"- {option}");
                    }
                }
                promptBuilder.AppendLine();
            }

            // Konversations historik
            if (request.ConversationHistory?.Any() == true)
            {
                promptBuilder.AppendLine("Tidigare:");
                foreach (var msg in request.ConversationHistory.TakeLast(3)) // Reducerad til 3
                {
                    var role = msg.Role == "user" ? "Användare" : "CyberChat";
                    promptBuilder.AppendLine($"{role}: {msg.Content}");
                }
                promptBuilder.AppendLine();
            }

            // Add current user message
            promptBuilder.AppendLine($"Användare: {request.UserMessage}");
            promptBuilder.AppendLine();
            promptBuilder.Append("CyberChat: ");

            return promptBuilder.ToString();
        }
    }
}
