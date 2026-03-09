using CyberQuiz_Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface IAiService
    {
        Task<AiCoachFeedbackDto> GenerateCoachFeedbackAsync(string userId, CancellationToken ct);
    }
}
