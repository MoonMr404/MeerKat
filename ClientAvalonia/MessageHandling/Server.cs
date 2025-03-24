using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MeerKatChatModule.Models.ServerModule
{
    public class Server
    {
        private bool _isRunning;
        private TcpListener _tcpListener;
        private ConcurrentDictionary<string, TcpClient> _clients; // Usa una stringa come chiave (nome utente)
        private static int Port { get; set; }
        private static string IpAddress { get; set; }

        public Server()
        {
            _clients = new ConcurrentDictionary<string, TcpClient>();
        }

        public static string GetServerHost() => IpAddress;

        public static int GetServerPort() => Port;

        public async Task StartServer(string ipAddress, int port)
        {
            try
            {
                IpAddress = ipAddress;
                IPAddress ip = IPAddress.Parse(ipAddress);
                _tcpListener = new TcpListener(ip, port);
                _tcpListener.Start();
                Console.WriteLine($"Server in ascolto su {ipAddress}:{port}...");

                _isRunning = true;
                while (_isRunning)
                {
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Formato IP errato: {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Errore durante l'avvio del server: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client) 
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            string username = null;

            try
            {
                username = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Nome utente non fornito. Chiusura della connessione.");
                return;
            }

            if (_clients.ContainsKey(username))
            {
                Console.WriteLine($"Nome utente già in uso: {username}");
                await writer.WriteLineAsync("ERROR: Nome utente già in uso.");
                return;
            }

            if (!_clients.TryAdd(username, client))
            {
                Console.WriteLine($"Impossibile aggiungere il client con nome utente: {username}");
                return;
            }

            Console.WriteLine($"Client connesso: {username}");

            // Invia la lista degli utenti connessi al nuovo client
            await SendUsersListAsync();

            while (client.Connected)
            {
                string message = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(message))
                {
                    break;
                }

                Console.WriteLine($"Ricevuto da {username}: {message}");
                await BroadcastMessageAsync(username, message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante la gestione del client: {ex.Message}");
        }
        finally
        {
            if (!string.IsNullOrEmpty(username) && _clients.ContainsKey(username))
            {
                _clients.TryRemove(username, out _);
            }

            try
            {
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la chiusura della connessione del client: {ex.Message}");
            }

            Console.WriteLine($"Client disconnesso: {username ?? "Sconosciuto"}");

            // Aggiorna la lista degli utenti connessi dopo la disconnessione
            await SendUsersListAsync();
        }
    }

        private async Task SendUsersListAsync()
        {
            // Crea un array con gli utenti connessi
            var usersArray = _clients.Keys.ToArray();

            // Loop attraverso ogni client e invia l'array
            foreach (var kvp in _clients)
            {
                try
                {
                    TcpClient recipientClient = kvp.Value;
                    if (recipientClient?.Connected ?? false)
                    {
                        NetworkStream stream = recipientClient.GetStream();
                        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                        // Serializza l'array di utenti in una stringa (ad esempio, JSON o CSV)
                        string usersList = string.Join(",", usersArray);
                        await writer.WriteLineAsync($"USERS:{usersList}");

                        Console.WriteLine($"Lista utenti inviata a {kvp.Key}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante l'invio della lista utenti a {kvp.Key}: {ex.Message}");
                }
            }
        }

        private async Task BroadcastMessageAsync(string sender, string message)
        {
            foreach (var kvp in _clients)
            {
                try
                {
                    // Non inviare il messaggio al mittente stesso
                    if (kvp.Key == sender) continue;

                    TcpClient recipientClient = kvp.Value;
                    if (recipientClient?.Connected ?? false)
                    {
                        NetworkStream stream = recipientClient.GetStream();
                        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                        // Invia il messaggio nel formato "nome_utente: messaggio"
                        await writer.WriteLineAsync($"{sender}: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante l'invio del messaggio a {kvp.Key}: {ex.Message}");
                }
            }
        }

        public void StopServer()
        {
            _isRunning = false;
            _tcpListener?.Stop();

            Console.WriteLine("Arresto del server...");

            foreach (var client in _clients.Values)
            {
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante la chiusura di una connessione client: {ex.Message}");
                }
            }

            _clients.Clear();

            Console.WriteLine("Server arrestato.");
        }
    }
}