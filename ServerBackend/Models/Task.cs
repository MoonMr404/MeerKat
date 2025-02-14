using System.ComponentModel.DataAnnotations;
using Shared.Dto;

namespace ServerBackend.Models;

public class Task
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(50)] 
    public string Description { get; set; }
    public Guid TaskListId { get; set; }
    public TaskList? TaskList { get; set; }
    
    [Required]
    public DateOnly Deadline { get; set; }
    
    public Task() { Id = Guid.NewGuid(); }

    public Task(string name, string description,Guid taskListId, DateOnly deadline,  TaskList? taskList = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Deadline = deadline;
        TaskListId = taskListId;
        TaskList = taskList;
    }
    
    //Dto 
    public static TaskDto ToDto(Task task)
    {
        var taskDto = new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Deadline = task.Deadline,
            TaskListId = task.TaskListId,
        };
        
        if (task.TaskList != null)
            taskDto.TaskList = TaskList.ToDto(task.TaskList);
        

        return taskDto;
    }
}