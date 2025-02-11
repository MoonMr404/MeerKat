using System;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
    
    //DTO
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _surname;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;
    [ObservableProperty] private string _passwordVerify;
    [ObservableProperty] private DateTime _dateOfBirth = DateTime.Today;
    
    private UserService userService;
    
    
    public RegistrationViewModel()
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
    }
   
    [RelayCommand]
    private void ChangePasswordVisibility()
    {
        PasswordVisible = !PasswordVisible;
    }

    [RelayCommand]
    private async void RegisterUser()
    {
        UserDto newUser = new UserDto
        {
            Name = _name,
            Surname = _surname,
            Email = _email,
            Password = _password,
            DateOfBirth = DateOnly.FromDateTime(_dateOfBirth),
        };
        
        Console.WriteLine(await userService.CreateUserAsync(newUser));
    }
}