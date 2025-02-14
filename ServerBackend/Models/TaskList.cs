using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Dto;

namespace ServerBackend.Models;

public class TaskList
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)] 
    public string Name { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string? Description { get; set; }
    public ICollection<Task>? Tasks { get; set; }
    public Guid TeamId { get; set; }
    public Team? Team { get; set; }
    public TaskList() {}

    public TaskList(string name, Guid teamId,Team? team = null, string? description = null, ICollection<Task>? tasks = null)
    {
        Name = name;
        Description = description;
        Tasks = tasks;
        TeamId = teamId;
        Team = team;
    }
    
    //Dto 
    public static TaskListDto ToDto(TaskList taskList, bool nested = false)
    {
        var taskListDto = new TaskListDto
        {
            Id = taskList.Id,
            Name = taskList.Name,
            Description = taskList.Description,
            TeamId = taskList.TeamId,
        };
        
        if (!nested) return taskListDto;
        taskListDto.Tasks = taskList.Tasks?.Select(t => Task.ToDto(t)).ToList();
        if (taskList.Team != null) taskListDto.Team = Team.ToDto(taskList.Team);
        return taskListDto;
    }

    public static TaskList FromDto(TaskListDto taskListDto)
    {
        var taskList = new TaskList
        {
            Id = taskListDto.Id,
            Name = taskListDto.Name,
            Description = taskListDto.Description,
            TeamId = taskListDto.TeamId,
        };

        return taskList;
    }
}