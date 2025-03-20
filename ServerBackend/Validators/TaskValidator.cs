using ServerBackend.Data;

namespace ServerBackend.Validators;

public static class TaskValidator
{
    public async static Task<bool> IsValid(this Models.Task? task)
    {
        if (task == null) return false;
        
        if (string.IsNullOrWhiteSpace(task.Name)) return false;
        if (string.IsNullOrWhiteSpace(task.Description)) return false;
        if (task.Deadline < DateOnly.FromDateTime(DateTime.Today)) return false;
        if (task.Status != "Da completare" && task.Status != "Consegnata" && task.Status != "Consegnata in ritardo") return false;

        return true;
    }
}