using System.ComponentModel.DataAnnotations;

namespace CyberQuiz.DAL.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}