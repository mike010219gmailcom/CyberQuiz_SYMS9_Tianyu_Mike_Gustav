using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Interfaces
{
    public interface ICategoryService
    {
        // Get/Read
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken ct = default);
    }
}
