using System.Text.Json.Serialization;

namespace Students.App.Models;

public class School
{
    public string TeacherId { get; set; } = string.Empty;

    [JsonConverter(typeof(StringOrStringArrayConverter))]
    public List<string> SubjectId { get; set; } = new();

    [JsonConverter(typeof(StringOrStringArrayConverter))]
    public List<string> SubjectIds { get; set; } = new();

    public List<string> ResolveSubjectIds()
    {
        return (SubjectId ?? new List<string>())
            .Concat(SubjectIds ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

public class SchoolResult
{
    public string TeacherId { get; set; } = string.Empty;
    public List<string> Added { get; set; } = new();
    public List<string> AlreadyLinked { get; set; } = new();
    public List<string> NotFound { get; set; } = new();
}

public class SchoolRemovedResult
{
    public string TeacherId { get; set; } = string.Empty;
    public List<string> Removed { get; set; } = new();
    public List<string> NotLinked { get; set; } = new();
}