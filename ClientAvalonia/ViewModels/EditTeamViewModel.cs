using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Dto;

namespace ClientAvalonia.ViewModels;

public partial class EditTeamViewModel : ViewModelBase
{
    [ObservableProperty] private bool _toggleEditName=false;
    [ObservableProperty] private bool _toggleEditDescriptionEdit=false;
    [ObservableProperty] private bool _toggleEditDeadlineEdit=false;
    
    
}

public class UserTemplate
{
    public UserDto user { get; }
    public UserTemplate(UserDto user)
    {
        this.user = user;
    }
    
}

