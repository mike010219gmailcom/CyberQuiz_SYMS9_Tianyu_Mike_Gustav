using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CyberQuiz.DAL.Models;

public class UserResult
{
    public int Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int SubCategoryId { get; set; }

    [ForeignKey(nameof(SubCategoryId))]
    public SubCategory SubCategory { get; set; } = null!;

    [Required]
    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public Question Question { get; set; } = null!;

    [Required]
    public int SelectedAnswerOptionId { get; set; }

    [ForeignKey(nameof(SelectedAnswerOptionId))]
    public AnswerOption SelectedAnswerOption { get; set; } = null!;

    [Required]
    public bool IsCorrect { get; set; }

    [Required]
    public DateTimeOffset AnsweredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public Guid QuizAttemptId { get; set; }
}