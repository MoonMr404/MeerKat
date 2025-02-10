using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Helpers;
using ServerBackend.Models;
using Shared.Dto;

namespace ServerBackend.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    MeerkatContext meerkatContext
    ) : ControllerBase
{
    private IQueryable<User> NestedTypes(bool nested) //Aggiunge i team se nested = true nella richiesta
    {
        var query = meerkatContext.Users.AsQueryable();

        if (!nested) return query;
        // Include le entità analoghe se nested = true
        query = query
            .Include(u => u.ManagedTeams)
            .Include(u => u.MemberOfTeams);
        return query;
    }
    
    //GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] bool nested = false)
    {
        var usersQuery = NestedTypes(nested);

        var users = await usersQuery.ToListAsync();
        var usersDtos = users.Select(u => Models.User.ToDto(u,nested)).ToList();
        
        return Ok( usersDtos ); //200
    }

    //GET: api/User/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, [FromQuery] bool nested = false)
    {
        var usersQuery = NestedTypes(nested);
        
        var user = await usersQuery.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound(); //404
        return Ok( Models.User.ToDto(user, nested) ); //200
    }
    
    //POST: api/User
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] User user)
    {
        user.Password = HashingHelper.Hash(user.Password);

        meerkatContext.Users.Add(user);
        if (!ModelState.IsValid) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, Models.User.ToDto(user)); // Returns 201 Created
    }
    
    // PUT: api/User
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        user.Password = HashingHelper.Hash(user.Password);
        
        meerkatContext.Users.Update(user);
        if (!ModelState.IsValid) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, Models.User.ToDto(user)); // 201
    }

    // DELETE: api/User/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await meerkatContext.Users.FindAsync(id);

        if (user == null) return NotFound(); // 404

        meerkatContext.Users.Remove(user);
        await meerkatContext.SaveChangesAsync();

        return NoContent(); // 204
    }
}