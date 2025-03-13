using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Helpers;
using ServerBackend.Models;
using Shared.Dto;

namespace ServerBackend.Controller;

[Route("api/[controller]")]
[ApiController]
public class TeamController(
    MeerkatContext meerkatContext
    ) : ControllerBase
{
    private IQueryable<Team> NestedTypes(bool nested) //Aggiunge gli utenti se nested = true nella richiesta
    {
        var query = meerkatContext.Teams.AsQueryable();

        if (!nested) return query;
        // Include le entità analoghe se nested = true
        query = query
            .Include(u => u.Manager)
            .Include(u => u.Members);
        return query;
    }
    
    //GET: api/Team
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams([FromQuery] bool nested = false)
    {
        if (!JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        var teamsQuery = NestedTypes(nested);

        var teams = await teamsQuery.ToListAsync();
        var teamDtos = teams.Select(t => Models.Team.ToDto(t, nested)).ToList();
        
        return Ok( teamDtos ); //200
    }

    //GET: api/Team/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetTeamById(Guid id, [FromQuery] bool nested = false)
    {
        var teamsQuery = NestedTypes(nested);
        
        var team = await teamsQuery.FirstOrDefaultAsync(t => t.Id == id);
        if (team == null) return NotFound(); //404
        return Ok( Models.Team.ToDto(team, nested) ); //200
    }
    
    //POST: api/Team
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] Team team)
    {
        if (!JwtHelper.IsAmongSelf(HttpContext.User,team.ManagerId) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        var manager = await meerkatContext.Users.FindAsync(team.ManagerId);
        
        if (manager == null) return BadRequest();
        
        team.Manager = manager;

        if (!ModelState.IsValid) { return BadRequest(ModelState); } //400
        meerkatContext.Teams.Add(team);
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, Team.ToDto(team)); // 201
    }
    
    // PUT: api/Team
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateTeam([FromBody] Team team)
    {
        if (!JwtHelper.IsAmongSelf(HttpContext.User,team.ManagerId) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        meerkatContext.Teams.Update(team);
        if (!ModelState.IsValid) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, Team.ToDto(team)); // 201
    }

    // DELETE: api/Team/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var team = await meerkatContext.Teams.FindAsync(id);

        if (team == null) return NotFound(); // 404
        
        if (!JwtHelper.IsAmongSelf(HttpContext.User,team.ManagerId) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();

        meerkatContext.Teams.Remove(team);
        await meerkatContext.SaveChangesAsync();

        return NoContent(); // 204
    }

    // POST: api/Team/addMember
    [HttpPost("{teamId}/addMember/{email}")]
    [Authorize]
    public async Task<IActionResult> AddMemberToTeam(string email, Guid teamId)
    {
        var team = await meerkatContext.Teams.FindAsync(teamId);
        if (team == null) return BadRequest(); //400
        var member = await meerkatContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (member == null) return BadRequest(); //400
        if (team.Members == null) team.Members = new List<User>();
        team.Members.Add(member);
        await meerkatContext.SaveChangesAsync();
        return NoContent(); // 204
    }
    
}