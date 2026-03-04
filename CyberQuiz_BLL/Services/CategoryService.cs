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
        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(
            CancellationToken ct = default)
        {
            var categories = await _categoryRepository
                .GetAllWithSubCategoriesAsync(ct);

                return categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                    }).ToList()
            })
            .ToList();
        }
    }
}
