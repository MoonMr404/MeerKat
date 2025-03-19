using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using ServerBackend.Helpers;
using ServerBackend.Models;

namespace UnitTesting.Tests.UnitTesting;

[TestFixture]
public class HelpersTests
{
    [Test]
    public void TC1_HashingAndVerifying()
    {
        var toHash = "didThisHash?";
        var toVerify = HashingHelper.Hash(toHash);
        
        Assert.That(HashingHelper.Verify(toHash,toVerify), Is.EqualTo(true));
    }

    [Test]
    public void TC2_AuthTokenGeneration()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MeerKat",
            ValidAudience = "mircats",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MeerKat è un software di gestione aziendale"))
        };
        
        var user = new User("Gabriel", "Tabasco", "greenylie12@gmail.com", "pollosacro", DateOnly.Parse("2025-01-05"));
        var token = JwtHelper.GenerateJwtToken(user);
        
        Assert.DoesNotThrow(() => tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken));
    }
}