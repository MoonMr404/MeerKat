using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Remote.Protocol.Input;
using Avalonia.Threading;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.Input;
using MeerKatChatModule.Models.ClientModule;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {


    
        
        public Dictionary<string, ObservableCollection<string>> Userchat { get; set; } = new();


        public ObservableCollection<string> Messages { get; set; } = new();

        public ICommand SendMessageCommand { get; }
        private readonly UserService userService;
        private readonly ChatClient _chatClient;
        private string currentUserName;
        private string _messageText;

        public string SelectedUser { get; set; }

        public string MessageText
        {
            get => _messageText;
            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    OnPropertyChanged(nameof(MessageText));
                }
            }
        }

        // UserDto u = (new UserDto()
        // {
        //     Name = "Gabirel",
        //     Email = " greenylie12@gmail.com",
        //     Surname = "Tabasco",
        //     Password = "password123",
        //     Id = Guid.NewGuid(),
        // });  
        //     
        // UserDto u2 = (new UserDto()
        // {
        //     Name = "John",
        //     Email = "john.doe@example.com",
        //     Surname = "Doe",
        //     Password = "password123",
        //     Id = Guid.NewGuid(),
        // });

        public ChatViewModel()
        {
            
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
            _chatClient = new ChatClient(this);
            _ =LoadUserAsync();
            _ = LoadConnectedUsers();

            // foreach (var user in Userchat)
            // {
            //     if (user.Key.Equals(currentUserName))
            //     {
            //         continue;
            //     }
            //     else
            //     {
            //         
            //     }
            //     
            //     
            //     Userchat.Add(u.Name, new ObservableCollection<string> { u.Name });
            //     Userchat.Add(u2.Name, new ObservableCollection<string> { u2.Name });
            //     
            // }
            

        }

        public async Task ConnectClientAsync()
        {
            try
            {
                var user = await userService.GetUserSelfAsync(true);
                currentUserName = user.Name;
                await _chatClient.ConnectAsync("127.0.0.1", 5002, currentUserName);
                Console.WriteLine("Connessione stabilita.");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore di connessione: {ex.Message}");
            }
        }

        public async Task LoadConnectedUsers()
        {
            foreach (var user in _chatClient.UsersConnected)
            {
                Userchat.Add(user, new ObservableCollection<string>()); // Empty initial messages for new users
                Console.WriteLine("UTENTI CONNESSI ", $"{user}");
            }
        }

        public async Task LoadUserAsync()
        {
            try
            {
                var user = await userService.GetUserSelfAsync(true);
                currentUserName = user.Name;
                Console.WriteLine("Utente caricato.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel caricamento utente: {ex.Message}");
            }
        }

        // Carica i messaggi dell'utente selezionato
        public async void LoadUserMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            // Aggiungi il messaggio all'interfaccia utente
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Add(message);

                if (SelectedUser == null && Userchat.Keys.Any()) // Controllo aggiunto
                {
                    SelectedUser = Userchat.Keys.First(); // Imposta il primo elemento
                }

                if (SelectedUser != null)
                {
                    if (!Userchat.ContainsKey(SelectedUser))
                    {
                        Userchat[SelectedUser] = new ObservableCollection<string>();
                    }

                    Userchat[SelectedUser].Add(message);
                }
                else
                {
                    Console.WriteLine("SelectedUser is null and Userchat is empty. Message not added to user chat.");
                    //gestione alternativa dell'errore.
                }
            });
        }

        // Carica i messaggi dell'utente selezionato
        public async void LoadMessageOnClick(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || !Userchat.ContainsKey(username))
            {
                Console.WriteLine($"Nessuna chat trovata con {username}.");
                return;
            }

            SelectedUser = username;

            // Carica i messaggi per l'utente selezionato
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Clear();
                foreach (var msg in Userchat[username])
                {
                    Messages.Add(msg);
                }
            });
        }

        // Invia il messaggio all'utente selezionato
        public async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText)) return;

            try
            {
                string fullMessage = $"{currentUserName}: {MessageText}";

                // Aggiungi il messaggio alla lista e forza l'aggiornamento UI
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LoadUserMessage(fullMessage);
                    OnPropertyChanged(nameof(Messages));  // Forza l'aggiornamento dell'interfaccia utente
                });

                // Invia il messaggio al server
                await _chatClient.SendMessageAsync(currentUserName, MessageText);

                // Svuota il campo di input del messaggio
                MessageText = string.Empty;
                OnPropertyChanged(nameof(MessageText));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'invio del messaggio: {ex.Message}");
            }
        }

        // Verifica se è possibile inviare un messaggio
        private bool CanSendMessage()
        {
            return !string.IsNullOrWhiteSpace(MessageText);
        }

        // Gestisce la ricezione di un messaggio
        public void ReceiveMessage(string rawMessage)
        {
            if (string.IsNullOrWhiteSpace(rawMessage))
            {
                Console.WriteLine("Messaggio ricevuto vuoto.");
                return;
            }

            Console.WriteLine($"Messaggio ricevuto: {rawMessage}"); // Debug

            var parts = rawMessage.Split(new[] { ": " }, 2, StringSplitOptions.None);
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            {
                Console.WriteLine($"Errore nel parsing del messaggio: {rawMessage}");
                return;
            }

            string username = parts[0].Trim();
            string message = parts[1].Trim();

            if (username == currentUserName) return; // Ignora i messaggi inviati dall'utente corrente

            // Aggiungi il messaggio all'interfaccia utente
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Add($"{message}");

                if (!Userchat.ContainsKey(username))
                {
                    Userchat[username] = new ObservableCollection<string>();
                }

                Userchat[username].Add($"{username}: {message}");
            });
        }

        // Evento per la notifica dei cambiamenti delle proprietà
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}