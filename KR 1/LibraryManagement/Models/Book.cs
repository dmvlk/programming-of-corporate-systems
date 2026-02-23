using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class Book
{
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
    
    public int AuthorId { get; set; }
    public int GenreId { get; set; }
    
    [ForeignKey("AuthorId")]
    public Author Author { get; set; }
    [ForeignKey("GenreId")]
    public Genre Genre { get; set; }
}