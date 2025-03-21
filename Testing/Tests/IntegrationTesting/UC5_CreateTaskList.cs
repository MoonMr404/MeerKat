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

public class UC5_CreateTaskList
{
    private MeerkatContext _context;
    private TaskListController _taskListController;

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

        var team = new Team
        {
            Name = "Team1",
            Description = "Team 1 description",
            Id = Guid.NewGuid(),
            ManagerId = user.Id,
        };
        
        _context.Users.Add(user);
        _context.Teams.Add(team);
        
        _context.SaveChanges();
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()) // Simula la richiesta da parte dello user
        }, "TestAuth"));
        
        _taskListController = new TaskListController(_context);
        
        _taskListController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
    
    [Test]
    public async Task CreateTaskList_TC1_SuccessfulCreation()
    {
        var taskList = new TaskList
        {
            Name = "Example TaskList",
            Description = "This task list was created as an example for testing.",
            TeamId = _context.Teams.FirstAsync().Result.Id
        };

        var result = await _taskListController.CreateTaskList(taskList);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        var taskListDto = createdResult.Value as TaskListDto;
        Assert.That(taskListDto, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTaskList_TC2_TitleTooLong()
    {
        var taskList = new TaskList
        {
            Name = "Example TaskList" + new string('x', 101),
            Description = "This task list was created as an example for testing.",
            TeamId = _context.Teams.FirstAsync().Result.Id
        };

        var result = await _taskListController.CreateTaskList(taskList);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTaskList_TC3_DescriptionTooLong()
    {
        var taskList = new TaskList
        {
            Name = "Example TaskList",
            Description = "This task list was created as an example for testing." + new string('x', 501),
            TeamId = _context.Teams.FirstAsync().Result.Id
        };

        var result = await _taskListController.CreateTaskList(taskList);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateTaskList_TC4_TitleAndDescriptionTooLong()
    {
        var taskList = new TaskList
        {
            Name = "Example TaskList" + new string('x', 101),
            Description = "This task list was created as an example for testing." + new string('x', 501),
            TeamId = _context.Teams.FirstAsync().Result.Id
        };

        var result = await _taskListController.CreateTaskList(taskList);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
}