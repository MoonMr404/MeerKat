using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ServerBackend.Controller;
using ServerBackend.Data;
using ServerBackend.Models;
using Shared.Dto;
using Task = System.Threading.Tasks.Task;

namespace Testing.Tests.IntegrationTesting;

public class UC1_UserRegistration
{
    private MeerkatContext _context;
    private UserController _userController;

    [SetUp]
    public void SetUp()
    {
        _context = InMemoryContext.GetMeerkatContext();
        if (_context.Database.IsInMemory())
            _context.Database.EnsureDeleted();
        
        _context.Users.Add(new User
        {
            Name = "Gabriel",
            Surname = "Tabasco",
            Email = "greenylie12@gmail.com", // Existing email
            Password = "Password1",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-24)
        });
        
        _context.SaveChanges();
        
        _userController = new UserController(_context);
    }
    
    [Test]
    public async Task CreateUser_TC1_SuccessfulRegistration()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "JohnDoe@gmail.com",
            Password = "Password1",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-24)
        };

        var result = await _userController.CreateUser(newUser);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        var userDto = createdResult.Value as UserDto;
        Assert.That(userDto, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateUser_TC2_DateOfBirthIncorrect()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "JohnDoe@gmail.com",
            Password = "Password1",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-1)
        };

        var result = await _userController.CreateUser(newUser);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateUser_TC3_PasswordTooShort()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "JohnDoe@gmail.com",
            Password = "Pass",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-24)
        };

        var result = await _userController.CreateUser(newUser);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
    
    [Test]
    public async Task CreateUser_TC4_EmailAlreadyExists()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "greenylie12@gmail.com", // Existing email
            Password = "Password1",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-24)
        };

        var result = await _userController.CreateUser(newUser);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }
}