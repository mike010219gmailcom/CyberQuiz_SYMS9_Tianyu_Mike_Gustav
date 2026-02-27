using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class AnswerOption
{
    public int Id { get; set; }

    [Required, MaxLength(400)]
    public required string Text { get; set; }

    public bool IsCorrect { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    public int Order { get; set; }
}