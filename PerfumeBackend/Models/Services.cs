using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfumeBackend.Models;

public class Service
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string[] Features { get; set; } = Array.Empty<string>();

    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}