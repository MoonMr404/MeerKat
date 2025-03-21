

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ServerBackend.Controller;
using ServerBackend.Data;
using ServerBackend.Models;
using Shared.Dto;
using Testing;
using Task = System.Threading.Tasks.Task;

public class UserControllerTests
{
    private MeerkatContext _context;
    private UserController _userController;
    private string adminEmail = "greenylie12@gmail.com";

    [SetUp]
    public void SetUp()
    {
        _context = InMemoryContext.GetMeerkatContext();
        if (_context.Database.IsInMemory())
            _context.Database.EnsureDeleted();
        
        _context.Users.Add(new User(
            "Gabriel",
            "Tabasco", 
            "greenylie12@gmail.com",
            "79FE079C4EE5E77F464737374FC421E4903EF5ECFCC3E0AC4300AB4B47732184:F5EC7341C374DE79B430B622EE59C7D6:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-20)));

        // Adding more users
        _context.Users.Add(new User(
            "Alice",
            "Wonderland",
            "alice.wonder@example.com",
            "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6:ABCDEF1234567890ABCDEF1234567890:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-25)));

        _context.Users.Add(new User(
            "Bob",
            "Builder",
            "bob.builder@example.com",
            "B1C2D3E4F5G6H7I8J9K0L1M2N3O4P5Q6R7S8T9U0V1W2X3Y4Z5:A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-30)));

        _context.Users.Add(new User(
            "Charlie",
            "Chocolate",
            "charlie.chocolate@example.com",
            "C1D2E3F4G5H6I7J8K9L0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4:CBA987654321FEDCBA987654321FEDCB:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-22)));

        _context.Users.Add(new User(
            "David",
            "Phoenix",
            "david.phoenix@example.com",
            "D1E2F3G4H5I6J7K8L9M0N1O2P3Q4R5S6T7U8V9W0X1Y2Z3:D9C8B7A6F5E4D3C2B1A0FEDCBA987654:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-28)));

        _context.Users.Add(new User(
            "Eve",
            "Sparkle",
            "eve.sparkle@example.com",
            "E1F2G3H4I5J6K7L8M9N0O1P2Q3R4S5T6U7V8W9X0Y1Z2:EVE123SPARKLE456EXAMPLE789HASH123:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-19)));

        _context.Users.Add(new User(
            "Frank",
            "Castle",
            "frank.castle@example.com",
            "F1G2H3I4J5K6L7M8N9O0P1Q2R3S4T5U6V7W8X9Y0Z1:FRANKCASTLEEXAMPLEHASH1234567890ABC:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-35)));

        _context.Users.Add(new User(
            "Grace",
            "Hopper",
            "grace.hopper@example.com",
            "G1H2I3J4K5L6M7N8O9P0Q1R2S3T4U5V6W7X8Y9Z0:GRACEHOPPERHASH1234567890EXAMPLEABC:50000:SHA256",
            DateOnly.FromDateTime(DateTime.Now).AddYears(-40)));
            
            _context.SaveChanges();
            
            _userController = new UserController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
    
    [Test]
    public async Task GetUsers_ReturnsOkResult_WhenUserIsAdmin()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Email, adminEmail) // Simula la richiesta da parte di un admin
        }, "TestAuth"));
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        
        var result = await _userController.GetUsers();
        
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var usersDtos = okResult.Value as List<UserDto>;
        Assert.That(usersDtos.Count, Is.EqualTo(8));
    }
    
    [Test]
    public async Task GetUsers_ReturnsOkResult_WhenUserIsNotAdmin()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Email, "alice.wonder@example.com") // Simula la richiesta da parte di un utente
        }, "TestAuth"));
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        
        var result = await _userController.GetUsers();
        
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }
    
    [Test]
    public async Task GetUserById_ReturnsOkResult_WhenUserIsAdminOrSelf()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, adminEmail) // Simulates a request from an admin
        }, "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _userController.GetUserById(_context.Users.First().Id);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var userDto = okResult.Value as UserDto;
        Assert.That(userDto, Is.Not.Null);
    }

    [Test]
    public async Task GetUserById_ReturnsUnauthorizedResult_WhenUserIsNotAdminOrSelf()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "alice.wonder@example.com") // Simulates a request from a non-admin user
        }, "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _userController.GetUserById(_context.Users.First().Id);
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }

    [Test]
    public async Task CreateUser_ReturnsCreatedResult_WithValidData()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "ciaoatuttibelliebrutti",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var result = await _userController.CreateUser(newUser);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);

        var userDto = createdResult.Value as UserDto;
        Assert.That(userDto, Is.Not.Null);
    }

    [Test]
    public async Task CreateUser_ReturnsBadRequestResult_WithInvalidData()
    {
        var newUser = new User
        {
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "", // Invalid password
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-25)
        };

        var result = await _userController.CreateUser(newUser);
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
    }

    [Test]
    public async Task UpdateUser_ReturnsOkResult_WithValidData_WhenUserIsAdmin()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Email, adminEmail) // Simula la richiesta da parte di un admin
        }, "TestAuth"));
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        var userToUpdate = _context.Users.First();
        userToUpdate.Name = "UpdatedName";
        userToUpdate.Surname = "UpdatedSurname";
        userToUpdate.Password = "UpdatedPasswordfff";

        var result = await _userController.UpdateUser(userToUpdate);
        var okResult = result as CreatedAtActionResult;
        Assert.That(okResult, Is.Not.Null);

        var updatedUserDto = okResult.Value as UserDto;
        Assert.That(updatedUserDto.Name, Is.EqualTo("UpdatedName"));
        Assert.That(updatedUserDto.Surname, Is.EqualTo("UpdatedSurname"));
    }
    
    [Test]
    public async Task UpdateUser_ReturnsOkResult_WithValidData_WhenUserIsItself()
    {
        var userToUpdate = _context.Users.First(u => u.Email == "alice.wonder@example.com");
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, userToUpdate.Id.ToString()) // Simula la richiesta da parte di un admin
        }, "TestAuth"));
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        userToUpdate.Name = "UpdatedName";
        userToUpdate.Surname = "UpdatedSurname";
        userToUpdate.Password = "UpdatedPasswordfff";

        var result = await _userController.UpdateUser(userToUpdate);
        var okResult = result as CreatedAtActionResult;
        Assert.That(okResult, Is.Not.Null);

        var updatedUserDto = okResult.Value as UserDto;
        Assert.That(updatedUserDto.Name, Is.EqualTo("UpdatedName"));
        Assert.That(updatedUserDto.Surname, Is.EqualTo("UpdatedSurname"));
    }

    [Test]
    public async Task UpdateUser_ReturnsUnauthorizedResult_WhenUserIsNotAdminOrSelf()
    {
        var userToUpdate = _context.Users.First();
        userToUpdate.Name = "UpdatedName";
        userToUpdate.Surname = "UpdatedSurname";

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "alice.wonder@example.com") // Simulates a request from a non-admin user
        }, "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _userController.UpdateUser(userToUpdate);
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }

    [Test]
    public async Task DeleteUser_ReturnsNoContentResult_WhenUserIsAdminOrSelf()
    {
        var userToDelete = _context.Users.First();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, adminEmail) // Simulates a request from an admin
        }, "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _userController.DeleteUser(userToDelete.Id);
        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
    }

    [Test]
    public async Task DeleteUser_ReturnsUnauthorizedResult_WhenUserIsNotAdminOrSelf()
    {
        var userToDelete = _context.Users.First();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "alice.wonder@example.com") // Simulates a request from a non-admin user
        }, "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _userController.DeleteUser(userToDelete.Id);
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }

    [Test]
    public async Task UserLogin_ReturnsOkResult_WithCorrectCredentials()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "greenylie12@gmail.com",
            Password = "pollosacro"
        };

        var result = await _userController.UserLogin(loginRequest);
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var userTokenDto = okResult.Value as UserTokenDto;
        Assert.That(userTokenDto, Is.Not.Null);
    }

    [Test]
    public async Task UserLogin_ReturnsUnauthorizedResult_WithIncorrectCredentials()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "greenylie12@gmail.com",
            Password = "incorrectpassword" // Incorrect password
        };

        var result = await _userController.UserLogin(loginRequest);
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }
}