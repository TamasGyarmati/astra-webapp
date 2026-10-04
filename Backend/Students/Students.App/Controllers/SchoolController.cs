using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.App.Data;
using Students.App.Models;

namespace Students.App.Controllers;

[ApiController]
[Route("api/school")]
public class SchoolController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    [ActionName(nameof(CreateSchool))]
    public async Task<ActionResult<SchoolResult>> CreateSchool(School body)
    {
        var subjectIds = body.ResolveSubjectIds();

        if (subjectIds.Count == 0)
        {
            return BadRequest();
        }

        var teacher = await db.Teachers
            .Include(x => x.TeachedSubjects)
            .FirstOrDefaultAsync(x => x.Id == body.TeacherId);

        if (teacher is null)
        {
            return NotFound();
        }

        var subjects = await db.Subjects
            .Where(x => subjectIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var result = new SchoolResult { TeacherId = teacher.Id };

        foreach (var subjectId in subjectIds)
        {
            if (!subjects.TryGetValue(subjectId, out var subject))
            {
                result.NotFound.Add(subjectId);
                continue;
            }

            if (teacher.TeachedSubjects.Any(x => x.Id == subject.Id))
            {
                result.AlreadyLinked.Add(subjectId);
                continue;
            }

            teacher.TeachedSubjects.Add(subject);
            result.Added.Add(subject.Id);
        }

        if (result.Added.Count == 0 && result.AlreadyLinked.Count > 0)
        {
            return Conflict(result);
        }

        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete]
    [ActionName(nameof(DeleteSchool))]
    public async Task<ActionResult<SchoolRemovedResult>> DeleteSchool(School body)
    {
        var subjectIds = body.ResolveSubjectIds();

        if (subjectIds.Count == 0)
        {
            return BadRequest();
        }

        var teacher = await db.Teachers
            .Include(x => x.TeachedSubjects)
            .FirstOrDefaultAsync(x => x.Id == body.TeacherId);

        if (teacher is null)
        {
            return NotFound();
        }

        var result = new SchoolRemovedResult { TeacherId = teacher.Id };

        foreach (var subjectId in subjectIds)
        {
            var subject = teacher.TeachedSubjects.FirstOrDefault(x => x.Id == subjectId);

            if (subject is null)
            {
                result.NotLinked.Add(subjectId);
                continue;
            }

            teacher.TeachedSubjects.Remove(subject);
            result.Removed.Add(subject.Id);
        }

        if (result.Removed.Count == 0)
        {
            return NotFound(result);
        }

        await db.SaveChangesAsync();

        return Ok(result);
    }
}