using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfumeBackend.Models;

public class Portfolio
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string[] Notes { get; set; } = Array.Empty<string>();

    public decimal Price { get; set; }
    public int Volume { get; set; }
    public string? ImageUrl { get; set; }
}