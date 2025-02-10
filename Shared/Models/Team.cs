using Shared.Utils;

namespace Shared.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public User Manager { get; set; }
    public Guid ManagerId { get; set; }
    public ICollection<User> Members { get; set; } = new HashSet<User>(); //Uso Hashset per evitare ripetizioni e perchè non serve un ordine preciso

    private string _name = null!;
    private DateTime? _deadline;
    private byte[]? _image;
    
    protected Team() {}

    public Team(string name, User manager, string? description = null, DateTime? deadline = null, byte[]? image = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Manager = manager;
        Description = description;
        Deadline = deadline;
        Image = image;
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
                throw new ArgumentException("Inserisci un nome per il team");
            }
            _name = value;
        } 
    }
    
    public DateTime? Deadline
    { 
        get => _deadline; 
        set 
        {

            if (value < DateTime.Now)
            {
                throw new ArgumentException("La data non può essere passata");
            }
                
            _deadline = value;
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
                _image = ImageManipulation.CropImage(value, 1024, 512);
            }
            catch (Exception ex) { throw new InvalidOperationException("Impossibile processare l'immagine.", ex); }
        } 
    }
}