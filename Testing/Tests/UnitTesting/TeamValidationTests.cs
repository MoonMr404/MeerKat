using System;

namespace Testing.Tests.UnitTesting;

using NUnit.Framework;
using ServerBackend.Data;
using ServerBackend.Models;
using ServerBackend.Validators;
using Task = System.Threading.Tasks.Task;

public class TeamValidationTests
{

    [Test]
    public async Task Validate_Team_ValidData_ShouldReturnTrue()
    {
        var team = new Team
        {
            Name = "Team1",
            ManagerId = Guid.NewGuid() //La presenza dell'id del manager è controllata dai tag dei modelli direttamente dall'ORM
        };

        var isValid = await TeamValidator.IsValid(team);
        Assert.That(isValid, Is.True);
    }

    [Test]
    public async Task Validate_Team_EmptyName_ShouldReturnFalse()
    {
        var team = new Team
        {
            Name = "",
            ManagerId = Guid.NewGuid()
        };

        var isValid = await TeamValidator.IsValid(team);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_Team_ManagerIdEmpty_ShouldReturnFalse()
    {
        var team = new Team
        {
            Name = "Team1",
            ManagerId = Guid.Empty
        };

        var isValid = await TeamValidator.IsValid(team);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_Team_PastDeadline_ShouldReturnFalse()
    {
        var team = new Team
        {
            Name = "Team1",
            ManagerId = Guid.NewGuid(),
            Deadline = DateTime.Now.AddDays(-1)
        };

        var isValid = await TeamValidator.IsValid(team);
        Assert.That(isValid, Is.False);
    }
}