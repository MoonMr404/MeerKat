using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
    
    //Dto
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;
    
    private UserService userService;
    public LoginViewModel()
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
    }
   
    [RelayCommand]
    private void ChangePasswordVisibility()
    {
        PasswordVisible = !PasswordVisible;
    }
    
    [RelayCommand]
    private async void LoginUser()
    {
        LoginRequestDto login = new LoginRequestDto
        {
            Email = Email,
            Password = Password,
        };
        
        Console.WriteLine((await userService.LoginAsync(login)).Token);
    }
}