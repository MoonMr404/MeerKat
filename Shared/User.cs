using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Shared;

public class User
{
    public Guid Id { get; private set; }
    
    // = null! è utilizzato per silenziare i warning, dato che che il setter si occupa di validare.
    private string _nome = null!;
    private string _cognome = null!;
    private string _email = null!;
    private string _hashedPassword = null!;
    private DateTime _dataNascita;
    private byte[] _immagine;

    public string Nome 
    { 
        get => _nome; 
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Nome non può essere vuoto.");
            }
            _nome = value;
        } 
    }

    public string Cognome 
    { 
        get => _cognome; 
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Nome non può essere vuoto.");
            }
            _cognome = value;
        } 
    }

    public string Email 
    { 
        get => _email; 
        set 
        {
            if (!IsValidEmail(value))
            {
                throw new ArgumentException("Email non valida.");
            }
            _email = value;
        } 
    }

    public string Password 
    { 
        get => _hashedPassword; 
        set 
        {
            if (string.IsNullOrEmpty(value) || value.Length < 8)
            {
                throw new ArgumentException("La password deve essere almeno di 8 caratteri.");
            }
            _hashedPassword = HashPassword(value);
        } 
    }

    public DateTime DataNascita 
    { 
        get => _dataNascita; 
        set 
        {
            if (value > DateTime.Today)
            {
                throw new ArgumentException("Si presuppone che tu sia già nato");
            }

            if (DateTime.Today.Year - value.Year < 18)
            {
                throw new ArgumentException("Devi avere almeno 18 anni per entrare in MeerKat");
            }
                
            _dataNascita = value;
        } 
    }
    public byte[] Immagine 
    { 
        get => _immagine; 
        set 
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentException("C'è stato un errore nel caricamento dell'immagine. Riprova.");
            }

            try
            {
                // TODO: Resizing da testare
                using var image = Image.Load(value);
                
                var originalWidth = image.Width;
                var originalHeight = image.Height;
                
                var cropSize = Math.Min(originalWidth, originalHeight);
                var startX = (originalWidth - cropSize) / 2;
                var startY = (originalHeight - cropSize) / 2;
                
                image.Mutate(x => x.Crop(new Rectangle(startX, startY, cropSize, cropSize)));
                
                image.Mutate(x => x.Resize(300, 300));
                
                using var ms = new System.IO.MemoryStream();
                image.SaveAsJpeg(ms);
                _immagine = ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Impossibile processare l'immagine.", ex);
            }
        } 
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string HashPassword(string password)
    {
        // TODO: Implement password hashing logic
        return null;
    }
}