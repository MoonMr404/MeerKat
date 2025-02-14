using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Models;

namespace ServerBackend.Validators;

public static class UserValidator
{
    public async static Task<bool> IsValid(this User? user, MeerkatContext meerkatContext)
    {
        if (user == null) return false;
        
        Regex emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        if (string.IsNullOrWhiteSpace(user.Name)) return false;
        if (string.IsNullOrWhiteSpace(user.Email) || !emailRegex.IsMatch(user.Email)) return false;
        if (string.IsNullOrWhiteSpace(user.Password)) return false;
        if (DateOnly.FromDateTime(DateTime.Today).Year - user.DateOfBirth.Year < 18) return false;
        if (await meerkatContext.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id)) return false;
        if (user.Password.Length < 8) return false;

        return true;
    }
}