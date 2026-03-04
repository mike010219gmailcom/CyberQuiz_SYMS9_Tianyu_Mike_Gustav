using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Repositories;
using CyberQuiz_BLL.DTOs;
using CyberQuiz_BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class CategoryService: ICategoryService
    {
        // Inject repository
        private readonly CategoryRepository _categoryRepository;
        private readonly QuizRepository _quizRepository;
        private readonly UserResultRepository _userResultRepository;
        public CategoryService(
            CategoryRepository categoryRepository,
            QuizRepository quizRepository,
            UserResultRepository userResultRepository)
        {
            _categoryRepository = categoryRepository;
            _quizRepository = quizRepository;
            _userResultRepository = userResultRepository;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(
            string userId,
            CancellationToken ct = default)
        {
            var categories = await _categoryRepository
                .GetAllWithSubCategoriesAsync(ct);

            foreach (var cat in categories)
            {
                var subs = cat.SubCategories.OrderBy(s => s.Order).ToList();

                for (int i = 0; i < cat.SubCategories.Count; i++)
                {
                    var sc = subs[i];
                    var questionCount = await _quizRepository.GetQuestionCountForSubCategoryAsync(sc.Id, ct);

                    if (i == 0)
                        sc.IsLocked = false; // first subcategory unlocked
                    else
                    {
                        var prev = subs[i - 1];
                        var prevAccuracy = await _userResultRepository
                            .GetAccuracyForUserInSubCategoryAsync(userId, prev.Id, ct);

                        sc.IsLocked = prevAccuracy < 0.8;
                    }
                }
                return categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        Order = sc.Order,
                        QuestionCount = sc.Questions.Count,
                        IsLocked = sc.IsLocked
                    }).ToList()
            })
            .ToList();
        }
    }
}
