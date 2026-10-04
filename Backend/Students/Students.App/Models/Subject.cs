using System.Text.Json.Serialization;

namespace Students.App.Models;

public class Subject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Neptun { get; set; } = string.Empty;
    public int Credit { get; set; }
    public bool Exam { get; set; }
    public string Image { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public int RegisteredStudents { get; set; }

    [JsonIgnore]
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
