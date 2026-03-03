using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using CyberQuiz_BLL.DTOs;
using CyberQuiz_Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyberQuiz_BLL.Services
{
    public class QuizService
    {
        private readonly CyberQuizDbContext _context;
        public QuizService(CyberQuizDbContext context)
        {
            _context = context;
        }

        // get question list
        public async Task<List<QuestionDto>> GetQuestionsAsync(int subCategoryId)
        {
            // load questions
            var questions = await _context.Questions
                .Where(q => q.SubCategoryId == subCategoryId)
                .Include(q => q.AnswerOptions) // load navigation property
                .AsNoTracking()
                .ToListAsync();

            // convert to List<QuestionDto> in memory
            var result = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Options = q.AnswerOptions
                    .Select(ao => new AnswerOptionDto
                    {
                        Id = ao.Id,
                        Text = ao.Text,
                    })
                    .ToList()
                })
                .ToList();

            return result;
        }

        // submitQuiz
        public async Task<QuizSummaryDto> SubmitQuizAsync(string userId, SubmitQuizDto dto)
        {
            var questions = await _context.Questions
                .Where(q => q.SubCategoryId == dto.SubCategoryId)
                .Include(q => q.AnswerOptions)
                .ToListAsync();

            int correctCount = 0;

            foreach (var answer in dto.Answers)
            {
                var question = questions.First(q => q.Id == answer.QuestionId);

                var correctOption = question.AnswerOptions
                    .First(a => a.IsCorrect);

                bool isCorrect = correctOption.Id == answer.SelectedAnswerOptionId;

                if (isCorrect)
                    correctCount++;

                var userResult = new UserResult
                {
                    UserId = userId,
                    SubCategoryId = dto.SubCategoryId,
                    QuestionId = answer.QuestionId,
                    SelectedAnswerOptionId = answer.SelectedAnswerOptionId,
                    IsCorrect = isCorrect,
                    AnsweredAtUtc = DateTime.UtcNow
                };

                _context.UserResults.Add(userResult);
            }

            await _context.SaveChangesAsync();

            double percentage = (double)correctCount / questions.Count * 100;

            return new QuizSummaryDto
            {
                TotalQuestions = questions.Count,
                CorrectAnswers = correctCount,
                ScorePercentage = percentage,
                CompletedAtUtc = DateTime.UtcNow
            };
        }

        // get user quiz history
        public async Task<UserQuizHistoryDto> GetUserQuizHistroryAsync(string userId)
        {
            var grouped = await _context.UserResults
                .Where(r => r.UserId == userId)
                .Include(r => r.SubCategory) // include navigation property
                .GroupBy(r => new { r.SubCategoryId, r.AnsweredAtUtc })
                .Select(g => new QuizSummaryDto
                {
                    SubCategoryName = g.First().SubCategory.Name,
                    TotalQuestions = g.Count(),
                    CorrectAnswers = g.Count(x => x.IsCorrect),
                    ScorePercentage = g.Count() == 0? 0 : (double)g.Count(x => x.IsCorrect) / g.Count() * 100,
                    CompletedAtUtc = g.Key.AnsweredAtUtc
                })
                .ToListAsync();

            return new UserQuizHistoryDto
            {
                UserId = userId,
                UserQuizHistory = grouped
            };
        }
    }
}
