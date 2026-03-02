using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public interface ICategoryService
    {
        // Get/Read
        Task<List<CategoryDto>> GetAllCategoryAsync();
    }
}
