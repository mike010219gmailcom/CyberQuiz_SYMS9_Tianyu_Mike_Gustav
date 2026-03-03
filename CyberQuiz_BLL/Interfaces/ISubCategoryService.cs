using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface ISubCategoryService
    {
        // Get/Read
        Task<List<SubCategoryDto>> GetSubCategoriesAsync(int categoryId);
    }
}
