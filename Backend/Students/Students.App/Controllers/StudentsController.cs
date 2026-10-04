using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.App.Data;
using Students.App.Models;

namespace Students.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(AppDbContext db) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName(nameof(GetStudentById))]
    public async Task<ActionResult<Student>> GetStudentById(string id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null)
        {
            return NotFound();
        }

        return student;
    }

    [HttpGet]
    [ActionName(nameof(GetStudents))]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        return await db.Students.ToListAsync();
    }

    [HttpPost]
    [ActionName(nameof(CreateStudent))]
    public async Task<ActionResult<Student>> CreateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Id))
        {
            student.Id = Guid.NewGuid().ToString();
        }

        db.Students.Add(student);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    [ActionName(nameof(UpdateStudent))]
    public async Task<IActionResult> UpdateStudent(string id, Student body)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null)
        {
            return NotFound();
        }

        student.Name = body.Name;
        student.IsActive = body.IsActive;
        student.BirthYear = body.BirthYear;
        student.Connections = body.Connections;
        student.CompletedCredits = body.CompletedCredits;
        student.ActiveSemesterCount = body.ActiveSemesterCount;
        student.Image = body.Image;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName(nameof(DeleteStudent))]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null)
        {
            return NotFound();
        }

        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
