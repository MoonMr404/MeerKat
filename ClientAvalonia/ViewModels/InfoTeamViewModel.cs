using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;

namespace ClientAvalonia.ViewModels;

public partial class InfoTeamViewModel : ViewModelBase
{
    [ObservableProperty] static private TeamDto _team;

    [ObservableProperty] private DateTime? _date = DateTime.Today; //_team.Deadline;
    [ObservableProperty] private bool _editName = false;
    [ObservableProperty] private bool _editDescription = false;
    [ObservableProperty] private bool _editDeadline = false;
    [ObservableProperty] private string _nameColor = "#313131";
    [ObservableProperty] private string _descriptionColor = "#313131";
    [ObservableProperty] private string _deadlineColor = "#313131";

    private ObservableCollection<UserTemplate> members { get; } = new ObservableCollection<UserTemplate>();

    [RelayCommand]
    public void toggleEditName()
    {
        EditName = !EditName;
        if (NameColor == "#717171") NameColor = "#313131";
        else NameColor = "#717171";
    }

    [RelayCommand]
    public void toggleEditDescription()
    {
        EditDescription = !EditDescription;
        if (DescriptionColor == "#717171") DescriptionColor = "#313131";
        else DescriptionColor = "#717171";
    }

    [RelayCommand]
    public void toggleEditDeadline()
    {
        EditDeadline = !EditDeadline;
        if (DeadlineColor == "#717171") DeadlineColor = "#313131";
        else DeadlineColor = "#717171";
    }
}


public class UserTemplate
{
    public UserDto user { get; }
    public UserTemplate(UserDto user)
    {
        this.user = user;
    }
    
}

