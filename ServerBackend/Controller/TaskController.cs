using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Helpers;
using ServerBackend.Models;
using Shared.Dto;

namespace ServerBackend.Controller;

[Route("api/[controller]")]
[ApiController]
public class TaskController(
    MeerkatContext meerkatContext
) : ControllerBase
{
    private IQueryable<Models.Task> NestedTypes(bool nested)
    {
        var query = meerkatContext.Task.AsQueryable();
        return (IQueryable<Models.Task>)query;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] bool nested = false)
    {
        var tasksQuery = NestedTypes(nested);
        var tasks = await tasksQuery.ToListAsync();
        var taskDtos = tasks.Select(t => Models.Task.ToDto(t)).ToList();
        
        return Ok(taskDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid id, [FromQuery] bool nested = false)
    {
        var tasksQuery = NestedTypes(nested);
        var task = await tasksQuery.FirstOrDefaultAsync(t => t.Id == id);
        if(task == null) return NotFound();
        return Ok(Models.Task.ToDto(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] Models.Task task)
    {
        task.Status = "Da completare";
        meerkatContext.Task.Add(task);
        if(!ModelState.IsValid) return BadRequest(ModelState);
        await meerkatContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, Models.Task.ToDto(task));
    }

    [HttpPut]
    public async Task<ActionResult<TaskDto>> UpdateTask([FromBody] Models.Task task)
    {
        meerkatContext.Task.Update(task);
        if(!ModelState.IsValid) return BadRequest(ModelState);
        await meerkatContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, Models.Task.ToDto(task));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await meerkatContext.Task.FindAsync(id);

        if (task == null) return NotFound(); // 404

        meerkatContext.Task.Remove(task);
        await meerkatContext.SaveChangesAsync();
        
        return NoContent(); // 204
    }
    
    [HttpGet("complete/{id}")]
    public async Task<ActionResult<TaskDto>> CompleteTask(Guid id)
    {
        var task = await meerkatContext.Task.FindAsync(id);
        if(task == null) return NotFound();
        var today = DateOnly.FromDateTime(DateTime.Now);
        if (task.Deadline < today)
        {
            task.Status = "Consegnata in ritardo";
        }
        else
        {
            task.Status = "Consegnata";
        }

        meerkatContext.Task.Update(task);
        if(!ModelState.IsValid) return BadRequest(ModelState);
        await meerkatContext.SaveChangesAsync();
        return Ok(Models.Task.ToDto(task));
    }
}