using CyberQuiz_BLL.DTOs;
using CyberQuiz_DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class CategoryService
    {
        private readonly CyberQuizDbContext _context;
        public CategoryService(CyberQuizDbContext context)
        {
            _context = context; 
        }

        public async Task<List<CategoryDto>> GetAllCategoryAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
            })
            .ToListAsync();
        }
    }
}
