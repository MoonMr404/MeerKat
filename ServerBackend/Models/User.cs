using System.ComponentModel.DataAnnotations;
using Shared.Dto;

namespace ServerBackend.Models;

public class User
{
    [Key]
    public Guid Id { get; set;  }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    public string Surname { get; set; }
    [Required]
    [MaxLength(320)]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public DateOnly DateOfBirth { get; set; }
    public byte[]? Image;
    public ICollection<Team>? ManagedTeams { get; set; }
    public ICollection<Team>? MemberOfTeams { get; set; }
    
    public User() { Id = Guid.NewGuid(); }

    public User(string name, string surname, string email, string password, DateOnly dateOfBirth, byte[]? image = null,
        ICollection<Team>? managedTeams = null, ICollection<Team>? memberOfTeams = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        DateOfBirth = dateOfBirth;
        Image = image;
        ManagedTeams = managedTeams;
        MemberOfTeams = memberOfTeams;
    }
    
    //Dto Mapping
    public static UserDto ToDto(User user, bool nested = false)
    {
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            Image = user.Image,
        };

        if (!nested) return userDto;
        
        userDto.ManagedTeams = user.ManagedTeams?.Select(t => Team.ToDto(t)).ToList();
        userDto.MemberOfTeams = user.MemberOfTeams?.Select(t => Team.ToDto(t)).ToList();

        return userDto;
    }
}