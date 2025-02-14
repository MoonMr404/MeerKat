using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClientAvalonia.ViewModels;

public partial class AuthenticationWindowViewModel : ViewModelBase
{
    public event EventHandler? LoginSuccess;
    
    [ObservableProperty] private ViewModelBase _currentPage = new RegistrationViewModel();
    [ObservableProperty] private String _buttonText="Accedi";
    
    

    public void ChangeContent()
    {
        if (CurrentPage is LoginViewModel)
        {
            CurrentPage = new RegistrationViewModel();
            ButtonText = "Accedi";
        } else if (CurrentPage is RegistrationViewModel)
        {
            ButtonText = "Registrati";
            CurrentPage = new LoginViewModel(LoginSuccess);
            
        }
    }
    
    
    
}
