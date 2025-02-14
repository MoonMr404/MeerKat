using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using ClientAvalonia.Views;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class TeamsViewModel : ViewModelBase
{

    public ObservableCollection<ExampleTeamTemplate> teamList { get; } = new()
    {
       new ExampleTeamTemplate("team1", "primo team di esempio creato"),
       new ExampleTeamTemplate("team2", "secondo team di esempio creato"),
    };
    
   
}


public class TeamTemplate
{
    public TeamDto team { get; }
    public TeamTemplate(TeamDto team)
    {
        this.team = team;
    }
    
}

public partial class ExampleTeamTemplate
{
    public string name { get; }
    public string description { get; }

    public ExampleTeamTemplate(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
    
    [RelayCommand]
    public void editTeam()
    {
        MainWindowViewModel window = new MainWindowViewModel();
        window.CurrentPage = new InfoTeamViewModel();
    }
    
}