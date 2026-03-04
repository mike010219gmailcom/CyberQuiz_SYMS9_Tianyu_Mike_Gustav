using CyberQuiz.DAL.Repositories;
using CyberQuiz_BLL.DTOs;
using CyberQuiz_BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class UserProgressService : IUserProgressService
    {
        // Inject repositories
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserResultRepository _userResultRepository;
        public UserProgressService(
            CategoryRepository categoryRepository,
            UserResultRepository userResultRepository)
        {
            _categoryRepository = categoryRepository;
            _userResultRepository = userResultRepository;

        }

        // get user quiz history
        public async Task<UserQuizHistoryDto> GetUserQuizHistoryAsync(string userId, int subCategoryId, CancellationToken ct = default)
        {
            // load all user results
            var results = await _userResultRepository
               .GetResultsForUserAndSubCategoryAsync(userId, subCategoryId, ct);

            // load all subCategories
            var allSubCategories = await _categoryRepository.GetAllWithSubCategoriesAsync(ct);

            // group by subCategoryId
            var history = results
                .GroupBy(r => r.SubCategoryId)
                .Select(g =>
                {
                    var subCategory = allSubCategories.FirstOrDefault(sc => sc.Id == g.Key);
                    int totalQuestions = g.Count();
                    int correctAnswers = g.Count(r => r.IsCorrect);
                    double scorePercentage = totalQuestions > 0
                        ? (double)correctAnswers / totalQuestions * 100
                        : 0;

                    return new QuizSummaryDto
                    {
                        SubCategoryId = g.Key,
                        SubCategoryName = subCategory?.Name ?? "Unknown", // get sc name
                        TotalQuestions = totalQuestions,
                        CorrectAnswers = correctAnswers,
                        ScorePercentage = scorePercentage,
                        CompletedAtUtc = g.Max(r => r.AnsweredAtUtc)
                    };
                }).ToList();



            return new UserQuizHistoryDto
            {
                UserId = userId,
                UserQuizHistory = history
            };
        }
    }
}
