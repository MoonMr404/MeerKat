using Shared.Utils;

namespace Shared.Entities;

public class User
{
    public Guid Id { get; private set; }
    public ICollection<Team> ManagedTeams { get; set; } = new List<Team>();
    public ICollection<Team> MemberOfTeams { get; set; } = new List<Team>();
    
    // = null! è utilizzato per silenziare i warning, dato che che il setter si occupa di validare.
    private string _name = null!;
    private string _surname = null!;
    private string _email = null!;
    private string _hashedPassword = null!;
    private DateOnly _birthDate;
    private byte[]? _image;
    
    protected User() {}
    
    public User(string name, string surname, string email, string password, DateOnly birthDate, byte[]? image = null)
    {
        //Se image è vuota il software gestisce con un fallback
        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        Image = image;
        BirthDate = birthDate;
    }
    
    /*
     * Validazione dei dati
     */

    public string Name 
    { 
        get => _name; 
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Nome non può essere vuoto.");
            }
            _name = value;
        } 
    }

    public string Surname 
    { 
        get => _surname; 
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cognome non può essere vuoto.");
            }
            _surname = value;
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
            _hashedPassword = SecretHasher.Hash(value);
        } 
    }

    public DateOnly BirthDate 
    { 
        get => _birthDate; 
        set 
        {

            if (DateTime.Today.Year - value.Year < 18)
            {
                throw new ArgumentException("Devi avere almeno 18 anni per entrare in MeerKat");
            }
                
            _birthDate = value;
        } 
    }
    public byte[]? Image 
    { 
        get => _image; 
        set 
        {
            if (value is null)
            {
                _image = null;
                return;
            }

            try
            {
                // TODO: Resizing da testare
                _image = ImageManipulation.CropImage(value, 300, 300);
            }
            catch (Exception ex) { throw new InvalidOperationException("Impossibile processare l'immagine.", ex); }
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
}