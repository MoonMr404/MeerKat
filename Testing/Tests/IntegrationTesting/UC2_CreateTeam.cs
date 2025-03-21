using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ServerBackend.Controller;
using ServerBackend.Data;
using ServerBackend.Models;
using Shared.Dto;
using Task = System.Threading.Tasks.Task;

namespace Testing.Tests.IntegrationTesting;

public class UC2_CreateTeam
{
    private MeerkatContext _context;
    private TeamController _teamController;

    [SetUp]
    public void SetUp()
    {
        _context = InMemoryContext.GetMeerkatContext();
        if (_context.Database.IsInMemory())
            _context.Database.EnsureDeleted();

        var user = new User
        {
            Name = "Gabriel",
            Surname = "Tabasco",
            Email = "greenylie12@gmail.com", // Existing email
            Password = "Password1",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-24)
        };
        
        _context.Users.Add(user);
        
        _context.SaveChanges();
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()) // Simula la richiesta da parte dello user
        }, "TestAuth"));
        
        _teamController = new TeamController(_context);
        
        _teamController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
    
    [Test]
    public async Task CreateTeam_TC1_SuccessfulCreation()
    {
        var team = new Team
        {
            Name = "Example Team",
            Description = "This team was created as an example for testing.",
            Deadline = DateTime.Now.AddYears(1),
            ManagerId = _context.Users.FirstAsync().Result.Id
        };

        var result = await _teamController.CreateTeam(team);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        var teamDto = createdResult.Value as TeamDto;
        Assert.That(teamDto, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTeam_TC2_DeadlineInPast()
    {
        var team = new Team
        {
            Name = "Example Team",
            Description = "This team was created as an example for testing.",
            Deadline = DateTime.Now.AddYears(-1),
            ManagerId = _context.Users.FirstAsync().Result.Id
        };

        var result = await _teamController.CreateTeam(team);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTeam_TC3_DescriptionTooLong()
    {
        var team = new Team
        {
            Name = "Example Team",
            Description = "This team was created as an example for testing. " + new string('x', 501),
            Deadline = DateTime.Now.AddYears(1),
            ManagerId = _context.Users.FirstAsync().Result.Id
        };

        var result = await _teamController.CreateTeam(team);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTeam_TC4_DescriptionTooLongAndDeadlineInPast()
    {
        var team = new Team
        {
            Name = "Example Team",
            Description = "This team was created as an example for testing. " + new string('x', 501),
            Deadline = DateTime.Now.AddYears(-1),
            ManagerId = _context.Users.FirstAsync().Result.Id
        };

        var result = await _teamController.CreateTeam(team);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
}