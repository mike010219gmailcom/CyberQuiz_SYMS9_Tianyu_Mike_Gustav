using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberQuiz.DAL.Models;

public class SubCategory
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int Order { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
