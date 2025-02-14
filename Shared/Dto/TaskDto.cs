namespace Shared.Dto;

public class TaskDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Guid TaskListId { get; set; }
    public TaskListDto? TaskList { get; set; }
    public required DateOnly Deadline { get; set; }
}