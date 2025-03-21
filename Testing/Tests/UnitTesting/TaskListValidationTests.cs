using System;
using NUnit.Framework;
using ServerBackend.Data;
using ServerBackend.Models;
using ServerBackend.Validators;
using Task = System.Threading.Tasks.Task;

namespace Testing.Tests.UnitTesting;

public class TaskListValidatorTests
{

    [Test]
    public async Task Validate_TaskList_ValidData_ShouldReturnTrue()
    {
        var taskList = new TaskList
        {
            Name = "TaskList1",
            TeamId = Guid.NewGuid()
        };

        var isValid = await TaskListValidator.IsValid(taskList);
        Assert.That(isValid, Is.True);
    }

    [Test]
    public async Task Validate_TaskList_EmptyName_ShouldReturnFalse()
    {
        var taskList = new TaskList
        {
            Name = "",
            TeamId = Guid.NewGuid()
        };

        var isValid = await TaskListValidator.IsValid(taskList);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_TaskList_EmptyTeamId_ShouldReturnFalse()
    {
        var taskList = new TaskList
        {
            Name = "TaskList1",
            TeamId = Guid.Empty
        };

        var isValid = await TaskListValidator.IsValid(taskList);
        Assert.That(isValid, Is.False);
    }
}