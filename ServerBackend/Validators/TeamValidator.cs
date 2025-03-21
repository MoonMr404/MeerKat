using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using ServerBackend.Models;

namespace ServerBackend.Validators;

public static class TeamValidator
{
    public async static Task<bool> IsValid(this Team? team)
    {
        if (team == null) return false;
        
        if (string.IsNullOrWhiteSpace(team.Name)) return false;
        if (team.ManagerId == Guid.Empty) return false;
        if (team.Deadline is not null && DateTime.Today > team.Deadline ) return false;
        if (team.Description is not null && team.Description.Length > 500) return false;

        return true;
    }
}