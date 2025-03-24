using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Diagnostics; // Aggiungi questa riga

namespace ClientAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}