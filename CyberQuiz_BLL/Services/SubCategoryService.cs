using CyberQuiz_BLL.DTOs;
using CyberQuiz_DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class SubCategoryService
    {
        private readonly CyberQuizDbContext _context;
        public SubCategoryService(CyberQuizDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubCategoryDto>> GetAllSubCategoryAsync()
        {
            // DbContext only includes Categories
            // Show subCategory with condition CategoryId

        }
    }
}
