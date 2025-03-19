using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;
using Exception = System.Exception;

namespace ClientAvalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _errorVisible;
    [ObservableProperty] private bool userLoggedIn=false;
    
    public event EventHandler? LoginSuccess;
    
    //Dto
    [ObservableProperty] private string _email = "greenylie12@gmail.com";
    [ObservableProperty] private string _password = "pollosacro";
    
    private UserService userService;
    public LoginViewModel(EventHandler? onLoginSuccess)
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        LoginSuccess = onLoginSuccess;
    }
   
    [RelayCommand]
    private void ChangePasswordVisibility()
    {
        PasswordVisible = !PasswordVisible;
    }
    
    [RelayCommand]
    private async void LoginUser()
    {
        ErrorVisible = !(await IsDataValid());
        if (ErrorVisible) return;
        
        LoginRequestDto login = new LoginRequestDto
        {
            Email = Email,
            Password = Password,
        };
        try
        {
            await userService.LoginAsync(login);
            userLoggedIn = true;
            LoginSuccess?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ErrorVisible = true;
        }
    }

    
    private async Task<bool> IsDataValid()
    {
        ErrorMessage = "Riempi tutti i campi";
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) return false;
        
        Regex emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (!emailRegex.IsMatch(Email))
        {
            ErrorMessage = "Email non valida";
            return false;
        };
        if (Password.Length < 8)
        {
            ErrorMessage = "La password deve essere di almeno 8 caratteri";
            return false;
        }
        return true;
    }
    
}
