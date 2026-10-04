namespace Students.App.Models;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int BirthYear { get; set; }
    public int Connections { get; set; }
    public int CompletedCredits { get; set; }
    public int ActiveSemesterCount { get; set; }
    public string Image { get; set; } = string.Empty;
}
