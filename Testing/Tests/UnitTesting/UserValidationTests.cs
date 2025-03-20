using System;
using NUnit.Framework;
using ServerBackend.Data;
using ServerBackend.Models;
using ServerBackend.Validators;
using UnitTesting;
using Task = System.Threading.Tasks.Task;

public class UserValidatorTests
{
    private MeerkatContext _context;

    [SetUp]
    public void Setup()
    {
        _context = InMemoryContext.GetMeerkatContext();
    }

    [Test]
    public async Task Validate_User_ValidData_ShouldReturnTrue()
    {
        var user = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.True);
    }

    [Test]
    public async Task Validate_User_EmptyName_ShouldReturnFalse()
    {
        var user = new User
        {
            Name = "",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_User_InvalidEmail_ShouldReturnFalse()
    {
        var user = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "invalid-email",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_User_PasswordTooShort_ShouldReturnFalse()
    {
        var user = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "short",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_User_Underage_ShouldReturnFalse()
    {
        var user = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-17)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task Validate_User_DuplicateEmail_ShouldReturnFalse()
    {
        _context.Users.Add(new User
        {
            Name = "Existing",
            Surname = "User",
            Email = "existing.user@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        });
        await _context.SaveChangesAsync();

        var user = new User
        {
            Name = "Duplicate",
            Surname = "User",
            Email = "existing.user@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var isValid = await UserValidator.IsValid(user, _context);
        Assert.That(isValid, Is.False);
    }
}