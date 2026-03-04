using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface IUserProgressService
    {
        Task<UserQuizHistoryDto> GetUserQuizHistoryAsync(string userId, int subCategoryId, CancellationToken ct = default);
    }
}
