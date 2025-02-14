namespace Shared.Dto;

public class TaskListDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public ICollection<TaskDto>? Tasks { get; set; }
    public required Guid TeamId { get; set; }
    public TeamDto? Team{ get; set; }
}