using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientAvalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
   
    [RelayCommand]
    private void ChangePasswordVisibility()
    {
        PasswordVisible = !PasswordVisible;
    }
}