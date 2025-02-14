using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Models;
using Shared.Dto;

namespace ServerBackend.Controller;

[Route("api/[controller]")]
[ApiController]
public class TaskListController(
    MeerkatContext meerkatContext
    ) : ControllerBase
{
    private IQueryable<TaskList> NestedTypes(bool nested)
    {
        var query = meerkatContext.TaskList.AsQueryable(); 
        
        if(!nested) return query;

        query = query
            .Include(t => t.Team)
            .Include(t => t.Tasks);
        return query;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskListDto>>> GetTaskLists([FromQuery] bool nested = false)
    {
        var taskListsQuery = NestedTypes(nested);
        
        var taskLists = await taskListsQuery.ToListAsync();
        
        var taskListDtos = taskLists.Select(t => Models.TaskList.ToDto(t, nested)).ToList();
        
        return Ok(taskListDtos); //200
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskListDto>> GetTaskListById(Guid id, [FromQuery] bool nested = false)
    {
        var taskListsQuery = NestedTypes(nested);

        var taskList = await taskListsQuery.FirstOrDefaultAsync(t => t.Id == id);
        if(taskList == null) return NotFound();
        return Ok(Models.TaskList.ToDto(taskList, nested));
    }

    // POST: api/TaskList
    [HttpPost]
    public async Task<ActionResult<TaskListDto>> CreateTaskList([FromBody] TaskList taskList)
    {
        var team = await meerkatContext.Teams.FindAsync(taskList.TeamId);
        if (team == null) return BadRequest("Il team specificato non esiste."); // 400

        taskList.Team = team;

        if (!ModelState.IsValid) {return BadRequest(ModelState); } //400
        meerkatContext.TaskList.Add(taskList);
        await meerkatContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetTaskListById), new { id = taskList.Id }, TaskList.ToDto(taskList));
    }
    
    // PUT: api/Team
    [HttpPut]
    public async Task<IActionResult> UpdateTeam([FromBody] TaskList taskList)
    {

        meerkatContext.TaskList.Update(taskList);
        if (!ModelState.IsValid) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTaskListById), new { id = taskList.Id }, TaskList.ToDto(taskList)); // 201
    }

    // DELETE: api/Team/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var taskList = await meerkatContext.TaskList.FindAsync(id);

        if (taskList == null) return NotFound(); // 404

        meerkatContext.TaskList.Remove(taskList);
        await meerkatContext.SaveChangesAsync();

        return NoContent(); // 204
    } 
}