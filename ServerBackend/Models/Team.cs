using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Dto;

namespace ServerBackend.Models;

public class Team
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime? Deadline;
    public byte[]? Image;
    [ForeignKey("ManagerId")]
    public Guid ManagerId { get; set; }
    public User? Manager { get; set; }
    public ICollection<User>? Members { get; set; }
    public ICollection<TaskList>? TaskList { get; set; }
    
    public Team() {}

    public Team(string name, Guid managerId, User manager, string? description = null, DateTime? deadline = null, byte[]? image = null,
        ICollection<User>? members = null,
        ICollection<TaskList>? taskList = null)
    {
        Name = name;
        Description = description;
        Deadline = deadline;
        Image = image;
        ManagerId = managerId;
        Manager = manager;
        Members = members;
        TaskList = taskList;
    }
    
    public static TeamDto ToDto(Team team, bool nested = false)
    {
        var teamDto = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            Deadline = team.Deadline,
            ManagerId = team.ManagerId,
            Image = team.Image,
        };

        if (!nested) return teamDto;

        if (team.Manager != null) teamDto.Manager = User.ToDto(team.Manager);
        teamDto.Members = team.Members?.Select(u => User.ToDto(u)).ToList();
        teamDto.TaskList = team.TaskList?.Select(t => Models.TaskList.ToDto(t)).ToList();

        return teamDto;
    }
    
    public static Team FromDto(TeamDto teamDto, bool nested = false)
    {
        var team = new Team
        {
            Id = teamDto.Id,
            Name = teamDto.Name,
            Description = teamDto.Description,
            Deadline = teamDto.Deadline,
            ManagerId = teamDto.ManagerId,
            Image = teamDto.Image,
        };
        return team;
    }
}