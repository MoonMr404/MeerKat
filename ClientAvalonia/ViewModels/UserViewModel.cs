using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    [ObservableProperty] private string _nameSurname = "John Doe";
    [ObservableProperty] private string _eMail = "JohnDoe@gmail.com";
    [ObservableProperty] private string _dateOfBirth = "01/01/1990";
    [ObservableProperty] private bool _editEnableName = false;
    [ObservableProperty] private bool _editEnableMail = false;
    [ObservableProperty] private bool _editEnableDate = false;
    [ObservableProperty] private string _nameColor = "#313131";
    [ObservableProperty] private string _mailColor = "#313131";
    [ObservableProperty] private string _dateColor = "#313131";

    private UserService userService;
    public UserViewModel()
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        
    }

    [RelayCommand]
    private void toggleEditName()
    {
        EditEnableName = !EditEnableName;
        if(NameColor == "#717171") NameColor = "#313131";
        else NameColor = "#717171";
    }
    
    [RelayCommand]
    private void toggleEditMail()
    {
        EditEnableMail = !EditEnableMail;
        if(MailColor == "#717171") MailColor = "#313131";
        else MailColor = "#717171";
    }
    
    [RelayCommand]
    private void toggleEditDate()
    {
        EditEnableDate = !EditEnableDate;
        if(DateColor == "#717171") DateColor = "#313131";
        else DateColor = "#717171";
    }
}