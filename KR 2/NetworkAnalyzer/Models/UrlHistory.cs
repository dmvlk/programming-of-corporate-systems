using System.ComponentModel.DataAnnotations;
namespace NetworkAnalyzer.Models;

public class UrlHistory
{
    public int Id { get; set; }
    [Required]
    public string Url { get; set; } = "";
    public DateTime CheckTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? IpAddress { get; set; }
    public string? HostName { get; set; }
}