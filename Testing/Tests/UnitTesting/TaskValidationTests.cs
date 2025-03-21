using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using ServerBackend.Validators;
using Task = System.Threading.Tasks.Task;

namespace Testing.Tests.UnitTesting;

public class TaskValidationTests
{

    [Test]
    public async Task Validate_Task_ValidData_ShouldReturnTrue()
    {
        var task = new ServerBackend.Models.Task
        {
            Name = "Task1",
            Description = "This is a task",
            Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Status = "Da completare", //Viene comunque settato dal backend
            TaskListId = Guid.NewGuid()
        };

        var isValid = await task.IsValid();
        Assert.That(isValid, Is.True);
    }

    [Test]
    public async Task Validate_Task_EmptyName_ShouldReturnFalse()
    {
        var task = new ServerBackend.Models.Task
        {
            Name = "",
            Description = "This is a task",
            Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Status = "Da completare",
            TaskListId = Guid.NewGuid()
        };

        var isValid = await task.IsValid();
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_Task_EmptyDescription_ShouldReturnFalse()
    {
        var task = new ServerBackend.Models.Task
        {
            Name = "Task1",
            Description = "",
            Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Status = "Da completare",
            TaskListId = Guid.NewGuid()
        };

        var isValid = await task.IsValid();
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_Task_PastDeadline_ShouldReturnFalse()
    {
        var task = new ServerBackend.Models.Task
        {
            Name = "Task1",
            Description = "This is a task",
            Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
            Status = "Da completare",
            TaskListId = Guid.NewGuid()
        };

        var isValid = await task.IsValid();
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_Task_InvalidStatus_ShouldReturnFalse()
    {
        var task = new ServerBackend.Models.Task
        {
            Name = "Task1",
            Description = "This is a task",
            Deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Status = "InvalidStatus",
            TaskListId = Guid.NewGuid()
        };

        var isValid = await task.IsValid();
        Assert.That(isValid, Is.False);
    }
}