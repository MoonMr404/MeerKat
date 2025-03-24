using Avalonia;
using System;
using System.Threading.Tasks;
using MeerKatChatModule.Models.ServerModule;

namespace ClientAvalonia
{
    sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs 
        // or any SynchronizationContext-reliant code before AppMain is called.
        [STAThread]
        
        
        public static void Main(string[] args)
        {
            
            // Avvia il server in un task separato
            Task.Run(() => new Server().StartServer("127.0.0.1", 5002));
            // Avvia l'app Avalonia
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}