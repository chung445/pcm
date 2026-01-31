using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Members")]
public class Member
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    public double RankLevel { get; set; } = 3.0;

    public MemberStatus Status { get; set; } = MemberStatus.Active;

    public int TotalMatches { get; set; } = 0;

    public int WinMatches { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Challenge> CreatedChallenges { get; set; } = new List<Challenge>();
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
