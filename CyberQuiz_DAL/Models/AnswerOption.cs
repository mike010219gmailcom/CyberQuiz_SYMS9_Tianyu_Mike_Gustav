using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class AnswerOption
{
    public int Id { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public Question Question { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }
}