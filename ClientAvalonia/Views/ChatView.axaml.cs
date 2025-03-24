using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientAvalonia.ViewModels;

using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MeerKatChatModule.Models.ClientModule;
using Shared.Dto;


namespace ClientAvalonia.Views
{
    public partial class ChatView : UserControl
    {
        
        private ChatClient _chatClient;
        private ChatViewModel _chatViewModel;
        public ChatView()
        {
            InitializeComponent();
            DataContext = new ChatViewModel(); // Imposta il DataContext al tuo ViewModel
            _chatViewModel = DataContext as ChatViewModel;
            _chatClient = new ChatClient(_chatViewModel);
            Task.Run(async () => await ConnectClient());
        }

        private async Task ConnectClient()
        {
            try
            {

                await _chatViewModel.ConnectClientAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la connessione: {ex.Message}");
            }
        }



        public void OnUserSelected(object sender, RoutedEventArgs e)
        {
            var selectedUser = (sender as ListBox)?.SelectedItem as string;

            if (!string.IsNullOrWhiteSpace(selectedUser))
            {
                // Passa il nome dell'utente selezionato al metodo nel ViewModel
                _chatViewModel.LoadMessageOnClick(selectedUser);
            }
        }
        
        // Command="{Binding SendMessageCommand}" 
        public void OnSendClick(object sender, RoutedEventArgs e)
        {
            var mesg = this.FindControl<TextBox>("MessageTextBox");
            _chatViewModel.SendMessageCommand.Execute(null);
            
            mesg.Text = string.Empty;
        }

        private void OnMessageTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                var mesg = this.FindControl<TextBox>("MessageTextBox");
                var viewModel = DataContext as ChatViewModel;
                viewModel?.SendMessageCommand.Execute(null);
                
                
                mesg.Text = string.Empty;
            }
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 
