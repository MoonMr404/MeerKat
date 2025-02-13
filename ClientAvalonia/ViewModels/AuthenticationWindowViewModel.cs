using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClientAvalonia.ViewModels;

public partial class AuthenticationWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentPage = new LoginViewModel();
    [ObservableProperty] private String _buttonText="Registrati";
    
    
    
    public void ChangeContent()
    {
        if (CurrentPage is LoginViewModel)
        {
            CurrentPage = new RegistrationViewModel();
            ButtonText = "Accedi";
        } else if (CurrentPage is RegistrationViewModel)
        {
            ButtonText = "Registrati";
            CurrentPage = new LoginViewModel();
            
        }
    }
    
    
    
}
