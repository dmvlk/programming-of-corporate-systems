using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Genre
{
    public Genre()
    {
        Name = "";
        Description = "";
        Books = new HashSet<Book>();
    }

    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    public ICollection<Book> Books { get; set; }
}