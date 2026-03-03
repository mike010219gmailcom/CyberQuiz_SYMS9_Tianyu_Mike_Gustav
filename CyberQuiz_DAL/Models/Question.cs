using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public int SubCategoryId { get; set; }

    [ForeignKey(nameof(SubCategoryId))]
    public SubCategory SubCategory { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}