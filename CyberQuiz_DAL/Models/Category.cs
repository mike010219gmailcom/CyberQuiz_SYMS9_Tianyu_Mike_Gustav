using System.ComponentModel.DataAnnotations;

namespace CyberQuiz.DAL.Models;

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}