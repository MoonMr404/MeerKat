using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientAvalonia.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    [ObservableProperty] private bool _passwordVisible;
   
    [RelayCommand]
    private void ChangePasswordVisibility()
    {
        PasswordVisible = !PasswordVisible;
    }
}