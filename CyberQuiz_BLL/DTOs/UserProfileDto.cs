namespace CyberQuiz_BLL.DTOs;

public class UserProfileDto
{
    public int TotalAnswers { get; set; }
    public int TotalCorrect { get; set; }
    public double TotalAccuracy { get; set; }
    public int TotalAccuracyPercent { get; set; }
    public List<SubCategoryProgressDto> PerSubCategory { get; set; } = new();
}

public class SubCategoryProgressDto
{
    public int SubCategoryId { get; set; }
    public string SubCategoryName { get; set; } = "";
    public double Accuracy { get; set; }
    public int AccuracyPercent { get; set; }
}
