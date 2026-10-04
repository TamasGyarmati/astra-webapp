using System.Text.Json.Serialization;

namespace Students.App.Models;

public class Teacher
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Neptun { get; set; } = string.Empty;
    public int BirthYear  { get; set; }
    public string Image { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;

    public ICollection<Subject> TeachedSubjects { get; set; } = new List<Subject>();
}