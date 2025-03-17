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

        CreateTaskListButton.Click += CreateTaskListButtonClick;
        CancelPuButton.Click += CancelPuButtonClick;
    }

    private void CancelPuButtonClick(object? sender, RoutedEventArgs e)
    {
        CreateTaskList.IsOpen = false;
        NamePuBox.Text = string.Empty;
        DescriptionPuBox.Text = string.Empty;
    }

    private void CreateTaskListButtonClick(object sender, RoutedEventArgs e)
    {
        CreateTaskList.IsOpen = true;
    }
}