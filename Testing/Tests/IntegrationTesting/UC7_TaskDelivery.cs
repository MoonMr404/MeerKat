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

public class UC7_TaskDelivery
{
    private MeerkatContext _context;
    private TaskController _taskController;

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

        var taskList = new TaskList()
        {
            Id = Guid.NewGuid(),
            Name = "TaskList",
            Description = "TaskList description",
            TeamId = team.Id
        };

        var task = new ServerBackend.Models.Task()
        {
            Id = Guid.NewGuid(),
            Name = "Task",
            Description = "Task description",
            Deadline = DateOnly.FromDateTime(DateTime.Now),
            Status = "Da consegnare"
        };
        
        _context.Users.Add(user);
        _context.Teams.Add(team);
        _context.TaskList.Add(taskList);
        _context.Task.Add(task);
        
        _context.SaveChanges();
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()) // Simula la richiesta da parte dello user
        }, "TestAuth"));
        
        _taskController = new TaskController(_context);
        
        _taskController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
    
    [Test]
    public async Task CompleteTask_TC1_OnTimeDelivery()
    {
        var task = await _context.Task.FirstAsync();
        task.Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        _context.Update(task);
        await _context.SaveChangesAsync();

        var result = await _taskController.CompleteTask(task.Id);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var updatedTaskDto = okResult.Value as TaskDto;
        Assert.That(updatedTaskDto.Status, Is.EqualTo("Consegnata"));
    }
    
    [Test]
    public async Task CompleteTask_TC2_LateDelivery()
    {
        var task = await _context.Task.FirstAsync();
        task.Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
        _context.Update(task);
        await _context.SaveChangesAsync();

        var result = await _taskController.CompleteTask(task.Id);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var updatedTaskDto = okResult.Value as TaskDto;
        Assert.That(updatedTaskDto.Status, Is.EqualTo("Consegnata in ritardo"));
    }
}