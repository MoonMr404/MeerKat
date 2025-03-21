using ServerBackend.Data;
using ServerBackend.Models;

namespace ServerBackend.Validators;

public static class TaskListValidator
{
    public async static Task<bool> IsValid(this TaskList? taskList)
    {
        if (taskList == null) return false;
        
        if (string.IsNullOrWhiteSpace(taskList.Name) || taskList.Name.Length > 100) return false;
        if (taskList.Description is not null && taskList.Description.Length > 500) return false;
        if (taskList.TeamId == Guid.Empty) return false;

        return true;
    }
}