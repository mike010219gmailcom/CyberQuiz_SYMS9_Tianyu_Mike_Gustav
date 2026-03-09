using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface IUserProgressService
    {
        Task<UserQuizHistoryDto> GetQuizHistoryForSubCategoryAsync(string userId, int subCategoryId, CancellationToken ct = default);

        // NEW: Get full user profile with all subcategories
        Task<UserProfileDto> GetFullUserProfileAsync(string userId, CancellationToken ct = default);
    }
}
