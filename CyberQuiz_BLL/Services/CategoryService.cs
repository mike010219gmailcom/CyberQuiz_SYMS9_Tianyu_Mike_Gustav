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
            var categories = await _categoryRepository.GetAllWithSubCategoriesAsync(ct);

            var categoryDtos = new List<CategoryDto>();

            foreach (var cat in categories)
            {
                // Order subcategories
                var subs = cat.SubCategories.OrderBy(s => s.Order).ToList();
                var subDtos = new List<SubCategoryDto>();

                for (int i = 0; i < subs.Count; i++)
                {
                    var scEntity = subs[i];

                    var scDto = new SubCategoryDto
                    {
                        Id = scEntity.Id,
                        Name = scEntity.Name,
                        Order = scEntity.Order,
                        QuestionCount = await _quizRepository.GetQuestionCountForSubCategoryAsync(scEntity.Id, ct)
                    };

                    if (i == 0)
                        scDto.IsLocked = false; // first subcategory unlocked
                    else
                    {
                        var prevAccuracy = await _userResultRepository
                            .GetAccuracyForUserInSubCategoryAsync(userId, subs[i - 1].Id, ct);

                        scDto.IsLocked = prevAccuracy < 0.8;
                    }

                    subDtos.Add(scDto);
                }

                categoryDtos.Add(new CategoryDto
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    SubCategories = subDtos
                });
            }

            return categoryDtos;
        }
    }
}
