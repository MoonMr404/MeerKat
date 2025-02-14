using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _errorVisible;
    
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
        ErrorVisible = !(await IsDataValid());
        if (ErrorVisible) return;
        
        UserDto newUser = new UserDto
        {
            Name = Name,
            Surname = Surname,
            Email = Email,
            Password = Password,
            DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
        };
        
        try
        {
            await userService.CreateUserAsync(newUser);
            
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
        if (string.IsNullOrEmpty(Name) ||
            string.IsNullOrEmpty(Surname) ||
            string.IsNullOrEmpty(Email) ||
            string.IsNullOrEmpty(Password) ||
            string.IsNullOrEmpty(PasswordVerify))
        {
            return false;
        }
        
        Regex emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (Name.Contains(' ') || Surname.Contains(' '))
        {
            ErrorMessage = "Il nome e il cognome devono essere privi di spazi";
            return false;
        }
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
        if (Password != PasswordVerify)
        {
            ErrorMessage = "Le password devono corrispondere";
            return false;
        }

        if ((DateTime.Today.AddTicks(-DateOfBirth.Ticks).Year - 1) < 18)
        {
            ErrorMessage = "Devi avere almeno 18 anni per entrare in Meerkat";
            return false;
        }
        return true;
    }
}