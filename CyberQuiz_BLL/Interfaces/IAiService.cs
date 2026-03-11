using CyberQuiz_Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{

    // Interface för AI-tjänster, som att generera coachfeedback och hantera chattinteraktioner.
    public interface IAiService
    {
        Task<AiCoachFeedbackDto> GenerateCoachFeedbackAsync(string userId, CancellationToken ct);
        Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken ct);
    }
}
