using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class UserResult
{
    public int Id { get; set; }


    [Required, MaxLength(450)]
    public required string UserId { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }


    [ForeignKey(nameof(SelectedAnswerOption))]
    public int SelectedAnswerOptionId { get; set; }
    public AnswerOption? SelectedAnswerOption { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime AnsweredAtUtc { get; set; } = DateTime.UtcNow;

    public int SubCategoryId { get; set; }
}