using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Bookings")]
public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CourtId { get; set; }

    [ForeignKey("CourtId")]
    public Court? Court { get; set; }

    [Required]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    public Member? Member { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
