namespace Shared.Dto;

public class TeamDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline;
    public byte[]? Image;
    public required Guid ManagerId { get; set; }
    public UserDto? Manager { get; set; }
    public ICollection<UserDto>? Members { get; set; }
    public ICollection<TaskListDto>? TaskList { get; set; }
}