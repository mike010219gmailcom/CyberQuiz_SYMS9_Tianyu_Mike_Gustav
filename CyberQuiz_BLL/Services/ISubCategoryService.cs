using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public interface ISubCategoryService
    {
        // Get/Read
        Task<List<SubCategoryDto>> GetSubCategoryAsync(int categoryId);
    }
}
