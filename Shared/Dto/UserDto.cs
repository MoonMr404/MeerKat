namespace Shared.Dto;

public class UserDto
{
    public Guid Id { get; set;  }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; } //Nullable visto che richiesta solo per la creazione dell'utente, mai restituita dal server
    public DateOnly DateOfBirth { get; set; }
    public byte[]? Image;
    public ICollection<TeamDto>? ManagedTeams { get; set; }
    public ICollection<TeamDto>? MemberOfTeams { get; set; }
}