using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Data;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    
    [ObservableProperty] private UserDto _user= new UserDto() {Name="", Email ="" , Surname ="" , Password ="" , Id = Guid.Empty, DateOfBirth = DateOnly.FromDateTime(DateTime.Today)};
    [ObservableProperty] private string _nameSurname = "John Doe";
    [ObservableProperty] private string _eMail = "JohnDoe@gmail.com";
    [ObservableProperty] private string _dateOfBirth = "01/01/1990";
    [ObservableProperty] private bool _editEnableName = false;
    [ObservableProperty] private bool _editEnableMail = false;
    [ObservableProperty] private bool _editEnableDate = false;
    [ObservableProperty] private bool _editEnableSurname = false; 
    [ObservableProperty] private string _nameColor = "#313131";
    [ObservableProperty] private string _mailColor = "#313131";
    [ObservableProperty] private string _dateColor = "#313131";
    [ObservableProperty]  private string _surnameColor= "#313131";


    private UserService userService;
    public UserViewModel(MainWindowViewModel mainWindowViewModel)
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        LoadUserAsync();
    }

    public async Task LoadUserAsync()
    {
        userService.GetUserSelfAsync();
        var user= await userService.GetUserSelfAsync();
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            User = user;
        });
    } 

    [RelayCommand]
    private void toggleEditName()
    {
        EditEnableName = !EditEnableName;
        if(NameColor == "#717171") NameColor = "#313131";
        else NameColor = "#717171";
    }
    
    [RelayCommand]
    private void toggleEditSurname()
    {
        
        EditEnableSurname = !EditEnableSurname;
        if(SurnameColor == "#717171") SurnameColor = "#313131";
        else SurnameColor = "#717171";
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

    [RelayCommand]
    private void saveEdit()
    {
        userService.UpdateUserAsync(new UserDto(){Name=_user.Name, Email =_user.Email , Surname =_user.Surname , Password = _user.Password , Id = _user.Id, DateOfBirth = _user.DateOfBirth});
    }
}