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
    
    public ObservableCollection<TeamTemplate> TeamList2 { get; set; } = new()
    {
        new TeamTemplate(new TeamDto(){ Id=Guid.NewGuid(), Name="team 1", Description = "Primo Team", ManagerId =Guid.NewGuid()}),
        new TeamTemplate(new TeamDto(){ Id=Guid.NewGuid(), Name="team 1", Description = "Primo Team", ManagerId =Guid.NewGuid()}),
    };
    
    private UserService userService;
    public TeamsViewModel()
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        LoadUserAsync();
    }
    
    public async Task LoadUserAsync()
    {
        userService.GetUserSelfAsync();
        var user= await userService.GetUserSelfAsync(true);
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
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