using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using CyberQuiz.DAL.Repositories;
using CyberQuiz_BLL.DTOs;
using CyberQuiz_BLL.Interfaces;
using CyberQuiz_BLL.Mappers;
using CyberQuiz_Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class QuizService: IQuizService
    {
        // Inject repositories
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IUserResultRepository _userResultRepository;
        public QuizService(
            CategoryRepository categoryRepository,
            QuizRepository quizRepository, 
            UserResultRepository userResultRepository)
        {
            _categoryRepository = categoryRepository;
            _quizRepository = quizRepository;
            _userResultRepository = userResultRepository;
            
        }

        // get question list
        public async Task<List<QuestionDto>> GetQuestionsAsync(int subCategoryId, string userId, CancellationToken ct = default)
        {
            var subCategory = await _quizRepository
                .GetSubCategoryWithQuestionsAsync(subCategoryId, ct);

            if(subCategory == null)
                throw new Exception("SubCategory not found");

            if (!subCategory.Questions.Any())
                throw new Exception("No questions available");

            // load questions
            return subCategory.Questions
                .Select(q => EntityToDtoMapper.MapQuestion(q))
                .ToList();
        }

        // submitQuiz
        public async Task<QuizSummaryDto> SubmitQuizAysnc(string userId, SubmitQuizDto dto, CancellationToken ct = default)
        {
            // load subcategory with questions
            var subCategory = await _quizRepository.GetSubCategoryWithQuestionsAsync(dto.SubCategoryId, ct);
            if (subCategory == null)
                throw new Exception("Subcategory not found");

            int correctCount = 0;

            foreach (var answer in dto.Answers)
            {
                var question = subCategory.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                var correctOption = question.AnswerOptions
                    .FirstOrDefault(a => a.IsCorrect);
                if(correctOption == null) continue;

                bool isCorrect = correctOption.Id == answer.SelectedAnswerOptionId; // match dto property

                if (isCorrect)
                    correctCount++;

                // save result via repository
                var userResult = new UserResult
                {
                    UserId = userId,
                    SubCategoryId = dto.SubCategoryId,
                    QuestionId = answer.QuestionId,
                    SelectedAnswerOptionId = answer.SelectedAnswerOptionId,
                    IsCorrect = isCorrect,
                    AnsweredAtUtc = DateTime.UtcNow
                };

                await _userResultRepository.AddResultAsync(userResult, ct);
            }

            double percentage = subCategory.Questions.Count > 0
                ? (double)correctCount / subCategory.Questions.Count * 100
                : 0;

            return new QuizSummaryDto
            {
                TotalQuestions = subCategory.Questions.Count,
                CorrectAnswers = correctCount,
                ScorePercentage = percentage,
                CompletedAtUtc = DateTime.UtcNow
            };
        }

        public async Task<QuizSummaryDto> GetQuizSummaryAsync(string userid, int subcategoryid)
        {
            var results = await _userResultRepository
                .GetResultsForUserAndSubCategoryAsync(userid, subcategoryid);

            int total = results.Count;
            int correct = results.Count(r => r.IsCorrect);

            double percentage = total == 0 ? 0 : (double)correct / total * 100;

            return new QuizSummaryDto
            {
                TotalQuestions = total,
                CorrectAnswers = correct,
                ScorePercentage = percentage
            };
        }

    }
}
