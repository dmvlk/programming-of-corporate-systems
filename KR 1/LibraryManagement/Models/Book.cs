using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class Book
{
    public Book()
    {
        Title = "";
        ISBN = "";
        Publisher = "";
        Authors = new HashSet<Author>();
        Genres = new HashSet<Genre>();
    }

    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(13)]
    public string ISBN { get; set; }
    
    public int PublishYear { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Publisher { get; set; }
    
    public int QuantityInStock { get; set; }
    
    public ICollection<Author> Authors { get; set; }
    public ICollection<Genre> Genres { get; set; }
    
    public string AuthorsDisplay => string.Join(", ", Authors.Select(a => a.FullName));
    public string GenresDisplay => string.Join(", ", Genres.Select(g => g.Name));
}