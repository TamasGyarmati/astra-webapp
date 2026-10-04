using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.App.Data;
using Students.App.Models;

namespace Students.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController(AppDbContext db) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName(nameof(GetSubjectById))]
    public async Task<ActionResult<Subject>> GetSubjectById(string id)
    {
        var subject = await db.Subjects
            .Include(x => x.Teachers)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
        {
            return NotFound();
        }

        return subject;
    }

    [HttpGet]
    [ActionName(nameof(GetSubjects))]
    public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
    {
        return await db.Subjects
            .Include(x => x.Teachers)
            .ToListAsync();
    }

    [HttpPost]
    [ActionName(nameof(CreateSubject))]
    public async Task<ActionResult<Subject>> CreateSubject(Subject subject)
    {
        if (string.IsNullOrWhiteSpace(subject.Id))
        {
            subject.Id = Guid.NewGuid().ToString();
        }

        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSubjectById),
            new { id = subject.Id },
            subject
        );
    }

    [HttpPut("{id}")]
    [ActionName(nameof(UpdateSubject))]
    public async Task<IActionResult> UpdateSubject(string id, Subject body)
    {
        var subject = await db.Subjects
            .Include(x => x.Teachers)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
        {
            return NotFound();
        }

        subject.Name = body.Name;
        subject.Neptun = body.Neptun;
        subject.Credit = body.Credit;
        subject.Exam = body.Exam;
        subject.Image = body.Image;
        subject.CreatorName = body.CreatorName;
        subject.RegisteredStudents = body.RegisteredStudents;

        subject.Teachers.Clear();

        foreach (var teacher in body.Teachers)
        {
            var existingTeacher = await db.Teachers.FindAsync(teacher.Id);

            if (existingTeacher is not null)
            {
                subject.Teachers.Add(existingTeacher);
            }
        }

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName(nameof(DeleteSubject))]
    public async Task<IActionResult> DeleteSubject(string id)
    {
        var subject = await db.Subjects.FindAsync(id);

        if (subject is null)
        {
            return NotFound();
        }

        db.Subjects.Remove(subject);
        await db.SaveChangesAsync();

        return NoContent();
    }
}