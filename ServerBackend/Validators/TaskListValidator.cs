using ServerBackend.Data;
using ServerBackend.Models;

namespace ServerBackend.Validators;

public static class TaskListValidator
{
    public async static Task<bool> IsValid(this TaskList? taskList)
    {
        if (taskList == null) return false;
        
        if (string.IsNullOrWhiteSpace(taskList.Name)) return false;
        if (taskList.TeamId == Guid.Empty) return false;

        return true;
    }
}