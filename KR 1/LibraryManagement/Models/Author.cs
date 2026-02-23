using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Author
{
    public Author()
    {
        FirstName = "";
        LastName = "";
        Country = "";
        Books = new HashSet<Book>();
    }

    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    [MaxLength(100)]
    public string Country { get; set; }
    
    public ICollection<Book> Books { get; set; }
}