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
 
    public ObservableCollection<TeamTemplate> managedTeamsList { get; set; } = new();
    public ObservableCollection<TeamTemplate> memberOfTeamsList { get; set; } = new();
    
    private UserService userService;
    private TeamService teamService;
    private UserDto userDto;
    private MainWindowViewModel mainWindowViewModel;
    public TeamsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        teamService = Locator.Current.GetService<TeamService>() ?? throw new InvalidOperationException();
        this.mainWindowViewModel = mainWindowViewModel;
        LoadUserAsync();
    }
    
    public async Task LoadUserAsync()
    {
        userService.GetUserSelfAsync(true);
        var user= await userService.GetUserSelfAsync(true);
        userDto = user;
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            managedTeamsList.Clear();
            memberOfTeamsList.Clear();
            foreach (TeamDto team in user.ManagedTeams)
            {
                managedTeamsList.Add(new TeamTemplate(team, mainWindowViewModel));
            }
            foreach (TeamDto team in user.MemberOfTeams)
            {
                memberOfTeamsList.Add(new TeamTemplate(team, mainWindowViewModel));
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
    public MainWindowViewModel mainWindowViewModel { get; }
    public TeamService teamService { get; }
    public TeamTemplate(TeamDto team, MainWindowViewModel mainWindowViewModel)
    {
        this.team = team;
        this.mainWindowViewModel = mainWindowViewModel;
        this.teamService = Locator.Current.GetService<TeamService>() ?? throw new InvalidOperationException();
    }
    
    [RelayCommand]
    public void editTeam()
    {
      
    }
    
    [RelayCommand]
    public async void selectTeam()
    {
        var teamNested = await teamService.GetTeamByIdAsync(team.Id, true);
        mainWindowViewModel.SelectedTeam = teamNested;
        mainWindowViewModel.SelectedListItem =
            mainWindowViewModel.Pages.Where(page => page.ModelType == typeof(TaskManagementViewModel)).First();
    }
}