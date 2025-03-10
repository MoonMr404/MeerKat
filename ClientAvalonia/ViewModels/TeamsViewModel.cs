using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClientAvalonia.Services;
using ClientAvalonia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class TeamsViewModel : ViewModelBase
{
 
    public ObservableCollection<TeamTemplate> teamList { get; set; } = new();
    
    private UserService userService;
    private TeamService teamService;
    private UserDto userDto;
    public TeamsViewModel()
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        teamService = Locator.Current.GetService<TeamService>() ?? throw new InvalidOperationException();
        LoadUserAsync();
    }
    
    public async Task LoadUserAsync()
    {
        userService.GetUserSelfAsync(true);
        var user= await userService.GetUserSelfAsync(true);
        userDto = user;
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            teamList.Clear();
            foreach (TeamDto team in user.ManagedTeams)
            {
                teamList.Add(new TeamTemplate(team));
            }
            foreach (TeamDto team in user.MemberOfTeams)
            {
                teamList.Add(new TeamTemplate(team));
            }
        });
    } 
    
    [RelayCommand]
    public async void addTeam()
    {
        TeamDto team = new TeamDto()
        {
            Id = Guid.NewGuid(),
            Name = "team 1",
            Description = "Primo Team",
            ManagerId = userDto.Id,
            Deadline = DateTime.Now.AddHours(1)
        };

        try
        {
            await teamService.CreateTeamAsync(team);
            await LoadUserAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
   
}


public partial class TeamTemplate
{
    public TeamDto team { get; }
    public TeamTemplate(TeamDto team)
    {
        this.team = team;
    }
    
    [RelayCommand]
    public void editTeam()
    {
      
    }
}