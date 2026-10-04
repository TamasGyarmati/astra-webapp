using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.App.Data;
using Students.App.Models;

namespace Students.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherController(AppDbContext db) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName(nameof(GetTeacherById))]
    public async Task<ActionResult<Teacher>> GetTeacherById(string id)
    {
        var teacher = await db.Teachers
            .Include(x => x.TeachedSubjects)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (teacher is null)
        {
            return NotFound();
        }

        return teacher;
    }

    [HttpGet]
    [ActionName(nameof(GetTeachers))]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
    {
        var teachers = await db.Teachers
            .Include(x => x.TeachedSubjects)
            .ToListAsync();

        foreach (var teacher in teachers)
        {
            foreach (var subject in teacher.TeachedSubjects)
            {
                subject.Teachers = new List<Teacher>();
            }
        }

        return teachers;
    }

    [HttpPost]
    [ActionName(nameof(CreateTeacher))]
    public async Task<ActionResult<Teacher>> CreateTeacher(Teacher teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher.Id))
        {
            teacher.Id = Guid.NewGuid().ToString();
        }

        var subjectIds = teacher.TeachedSubjects
            .Select(x => x.Id)
            .ToList();

        teacher.TeachedSubjects.Clear();

        foreach (var subjectId in subjectIds)
        {
            var subject = await db.Subjects.FindAsync(subjectId);

            if (subject is not null)
            {
                teacher.TeachedSubjects.Add(subject);
            }
        }

        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTeacherById),
            new { id = teacher.Id },
            teacher
        );
    }

    [HttpPut("{id}")]
    [ActionName(nameof(UpdateTeacher))]
    public async Task<IActionResult> UpdateTeacher(string id, Teacher body)
    {
        var teacher = await db.Teachers
            .Include(x => x.TeachedSubjects)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (teacher is null)
        {
            return NotFound();
        }

        teacher.Name = body.Name;
        teacher.Neptun = body.Neptun;
        teacher.BirthYear = body.BirthYear;
        teacher.Image = body.Image;
        teacher.CreatorName = body.CreatorName;

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName(nameof(DeleteTeacher))]
    public async Task<IActionResult> DeleteTeacher(string id)
    {
        var teacher = await db.Teachers.FindAsync(id);

        if (teacher is null)
        {
            return NotFound();
        }

        db.Teachers.Remove(teacher);
        await db.SaveChangesAsync();

        return NoContent();
    }
}