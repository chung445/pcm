using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Courts")]
public class Court
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
