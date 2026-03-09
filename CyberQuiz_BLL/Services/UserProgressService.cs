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
            ICategoryRepository categoryRepository,
            IUserResultRepository userResultRepository)
        {
            _categoryRepository = categoryRepository;
            _userResultRepository = userResultRepository;

        }

        // get user quiz history for a user + subCategory
        public async Task<UserQuizHistoryDto> GetQuizHistoryForSubCategoryAsync(string userId, int subCategoryId, CancellationToken ct = default)
        {
            // load all user results
            var results = await _userResultRepository
               .GetResultsForUserAndSubCategoryAsync(userId, subCategoryId, ct);
            
            if (!results.Any())
            {
                return new UserQuizHistoryDto
                {
                    UserId = userId,
                    UserQuizHistory = new List<QuizSummaryDto>()
                };
            }

            // group by subCategoryId
            var history = results
                .GroupBy(r => r.QuizAttemptId)
                .Select(g =>
                {
                    int totalQuestions = g.Count();
                    int correctAnswers = g.Count(r => r.IsCorrect);
                    double scorePercentage = totalQuestions > 0
                        ? (double)correctAnswers / totalQuestions * 100
                        : 0;

                    return new QuizSummaryDto
                    {
                        QuizAttemptId = g.Key,
                        SubCategoryId = g.First().SubCategoryId,
                        SubCategoryName = g.First().SubCategory?.Name ?? "Unknown", // get sc name
                        TotalQuestions = totalQuestions,
                        CorrectAnswers = correctAnswers,
                        ScorePercentage = scorePercentage,
                        CompletedAtUtc = g.Max(r => r.AnsweredAtUtc)
                    };
                })
                .OrderByDescending(a => a.CompletedAtUtc)
                .ToList();


            return new UserQuizHistoryDto
            {
                UserId = userId,
                UserQuizHistory = history
            };
        }

        // NEW: Get full user profile with all subcategories
        public async Task<UserProfileDto> GetFullUserProfileAsync(string userId, CancellationToken ct = default)
        {
            // Get all subcategories
            var allCategories = await _categoryRepository.GetAllWithSubCategoriesAsync(ct);
            var allSubCategories = allCategories.SelectMany(c => c.SubCategories).ToList();

            var perSubCategory = new List<SubCategoryProgressDto>();
            int totalAnswers = 0;
            int totalCorrect = 0;

            foreach (var sc in allSubCategories)
            {
                var results = await _userResultRepository
                    .GetResultsForUserAndSubCategoryAsync(userId, sc.Id, ct);

                if (results.Any())
                {
                    int answers = results.Count;
                    int correct = results.Count(r => r.IsCorrect);

                    totalAnswers += answers;
                    totalCorrect += correct;

                    double accuracy = answers > 0 ? (double)correct / answers : 0;
                    int accuracyPercent = (int)(accuracy * 100);

                    perSubCategory.Add(new SubCategoryProgressDto
                    {
                        SubCategoryId = sc.Id,
                        SubCategoryName = sc.Name,
                        Accuracy = accuracy,
                        AccuracyPercent = accuracyPercent
                    });
                }
            }

            double totalAccuracy = totalAnswers > 0 ? (double)totalCorrect / totalAnswers : 0;
            int totalAccuracyPercent = (int)Math.Round(totalAccuracy * 100);

            return new UserProfileDto
            {
                TotalAnswers = totalAnswers,
                TotalCorrect = totalCorrect,
                TotalAccuracy = totalAccuracy,
                TotalAccuracyPercent = totalAccuracyPercent,
                PerSubCategory = perSubCategory
            };
        }
    }
}
