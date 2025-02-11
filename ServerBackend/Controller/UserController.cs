using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerBackend.Data;
using ServerBackend.Helpers;
using ServerBackend.Models;
using ServerBackend.Validators;
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
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] bool nested = false)
    {
        if (!JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        var usersQuery = NestedTypes(nested);

        var users = await usersQuery.ToListAsync();
        var usersDtos = users.Select(u => Models.User.ToDto(u,nested)).ToList();
        
        return Ok( usersDtos ); //200
    }

    //GET: api/User/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, [FromQuery] bool nested = false)
    {
        if (!JwtHelper.IsAmongSelf(HttpContext.User,id) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        var usersQuery = NestedTypes(nested);
        
        var user = await usersQuery.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound(); //404
        return Ok( Models.User.ToDto(user, nested) ); //200
    }
    
    //POST: api/User
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] User user) //Register
    {
        user.Password = HashingHelper.Hash(user.Password);

        meerkatContext.Users.Add(user);
        if (!ModelState.IsValid || !(await user.IsValid(meerkatContext))) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, Models.User.ToDto(user)); // Returns 201 Created
    }
    
    // PUT: api/User
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        if (!JwtHelper.IsAmongSelf(HttpContext.User,user.Id) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        user.Password = HashingHelper.Hash(user.Password);
        
        meerkatContext.Users.Update(user);
        if (!ModelState.IsValid || !(await user.IsValid(meerkatContext))) { return BadRequest(ModelState); } //400
        await meerkatContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, Models.User.ToDto(user)); // 201
    }

    // DELETE: api/User/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (!JwtHelper.IsAmongSelf(HttpContext.User,id) && !JwtHelper.IsAdmin(HttpContext.User)) return Unauthorized();
        var user = await meerkatContext.Users.FindAsync(id);

        if (user == null) return NotFound(); // 404

        meerkatContext.Users.Remove(user);
        await meerkatContext.SaveChangesAsync();

        return NoContent(); // 204
    }
    
    //POST: api/User/login
    [HttpPost("login")]
    public async Task<IActionResult> UserLogin([FromBody] LoginRequestDto request)
    {
        User? loggedUser = await meerkatContext.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email);
        
        if (loggedUser == null) return NotFound(); //404
        
        Console.WriteLine($"Email: {loggedUser.Email} | Password: {loggedUser.Password}");
        
        // Verify the input password against the stored hash
        if (!HashingHelper.Verify(request.Password, loggedUser.Password))
            return Unauthorized(); // 401
        
        
        
        var token = JwtHelper.GenerateJwtToken(loggedUser);
        
        return Ok( new { token = token } );
        
    }
}