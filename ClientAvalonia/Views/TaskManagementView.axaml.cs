using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ClientAvalonia.Views;

public partial class TaskManagementView : UserControl
{
    public TaskManagementView()
    {
        InitializeComponent();

        CreateTaskListButton.Click += (object sender, RoutedEventArgs e) => CreateTaskList.IsOpen = true;
        PuTlCancelButton.Click += PuTlCancelButtonClick;
        PuTCancelButton.Click += PuTCancelButtonClick;
    }

    private void PuTlCancelButtonClick(object? sender, RoutedEventArgs e)
    {
        CreateTaskList.IsOpen = false;
        PuTlNameBox.Text = string.Empty;
        PuTlDescriptionBox.Text = string.Empty;
    }
    
    private void PuTCancelButtonClick(object? sender, RoutedEventArgs e)
    {
        CreateTask.IsOpen = false;
        PuTNameBox.Text = string.Empty;
        PuTDescriptionBox.Text = string.Empty;
        PuTDatePicker.Text = string.Empty;
    }
}