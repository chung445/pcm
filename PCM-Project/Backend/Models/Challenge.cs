using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Challenges")]
public class Challenge
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public ChallengeType Type { get; set; }

    public GameMode GameMode { get; set; } = GameMode.None;

    public ChallengeStatus Status { get; set; } = ChallengeStatus.Open;

    public int? Config_TargetWins { get; set; }

    public int CurrentScore_TeamA { get; set; } = 0;

    public int CurrentScore_TeamB { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal EntryFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrizePool { get; set; } = 0;

    [Required]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public Member? Creator { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
