using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input;
using ClientAvalonia.ViewModels;
using Shared.Dto;

namespace MeerKatChatModule.Models.ClientModule
{
    public class ChatClient
    {
        private TcpClient _tcpClient;
        private StreamReader _reader;
        private StreamWriter _writer;
        private ChatViewModel _chatViewModel;
        private bool _isConnected = false;

        public ObservableCollection<string> UsersConnected { get; set; } = new ObservableCollection<string>();
        public bool IsConnected { get => _isConnected; private set => _isConnected = value; }

        public ChatClient(ChatViewModel chatViewModel)
        {
            _chatViewModel = chatViewModel;
        }

        public async Task ConnectAsync(string host, int port, string username)
        {
            if (_isConnected)
            {
                Console.WriteLine("Sei già connesso al server.");
                return;
            }

            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(host, port);
                _isConnected = true;
                Console.WriteLine("Connesso al server.");

                NetworkStream stream = _tcpClient.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Invia il nome utente al server
                await _writer.WriteLineAsync(username);

                // Avvia la lettura dei messaggi in background
                _ = ReadMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la connessione: {ex.Message}");
                Disconnect();
            }
        }

        private async Task ReadMessagesAsync()
        {
            try
            {
                while (_tcpClient?.Connected == true)
                {
                    string message = await _reader.ReadLineAsync();
                    if (message == null) break;

                    message = message.Trim();

                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine($"Messaggio ricevuto: {message}");

                        // Verifica se il messaggio è un aggiornamento della lista degli utenti connessi
                        if (message.StartsWith("USERS:"))
                        {
                            UpdateUsersList(message);
                        }
                        else
                        {
                            // Aggiorna la UI con il messaggio generico
                            _chatViewModel.ReceiveMessage(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore READ: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        public void UpdateUsersList(string usersMessage)
        {
            try
            {
                // Rimuovi il prefisso "USERS:" e suddividi la stringa in base alla virgola
                string userList = usersMessage.Substring("USERS:".Length);
                string[] users = userList.Split(',');

                UsersConnected.Clear();
                foreach (string user in users)
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        UsersConnected.Add(user.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'aggiornamento della lista utenti: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string userName, string message)
        {
            if (_writer != null && _tcpClient?.Connected == true)
            {
                try
                {
                    string fullMessage = $"{userName}: {message}";
                    Console.WriteLine($"Invio messaggio al server: {fullMessage}");

                    await _writer.WriteLineAsync(fullMessage);

                    // Quando il messaggio è inviato, aggiorna la UI con il messaggio inviato
                    _chatViewModel.ReceiveMessage(fullMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante l'invio del messaggio: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Connessione al server non disponibile.");
            }
        }

        public void Disconnect()
        {
            try
            {
                _reader?.Dispose();
                _writer?.Dispose();
                _tcpClient?.Close();
                Console.WriteLine("Disconnesso dal server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la disconnessione: {ex.Message}");
            }
            finally
            {
                _isConnected = false; // Permette una nuova connessione futura
                _reader = null;
                _writer = null;
                _tcpClient = null;
            }
        }
    }
}