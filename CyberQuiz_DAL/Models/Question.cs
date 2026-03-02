using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class Question
{
    public int Id { get; set; }

    [Required, MaxLength(800)]
    public required string Text { get; set; }

    [ForeignKey(nameof(SubCategory))]
    public int SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}