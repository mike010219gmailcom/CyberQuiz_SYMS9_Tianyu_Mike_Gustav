using CyberQuiz.DAL.Models;
using CyberQuiz_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Mappers
{
    public static class EntityToDtoMapper
    {
            public static QuestionDto MapQuestion(Question q)
            {
                return new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.AnswerOptions.Select(a => new AnswerOptionDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                    }).ToList()
                };
            }

            public static CategoryDto MapCategory(Category c)
            {
                return new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name
                    }).ToList()
                };
            }
        }
    
}
