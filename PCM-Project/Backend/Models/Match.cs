using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Matches")]
public class Match
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsRanked { get; set; } = true;

    public int? ChallengeId { get; set; }

    [ForeignKey("ChallengeId")]
    public Challenge? Challenge { get; set; }

    public MatchFormat MatchFormat { get; set; }

    // Team 1
    [Required]
    public int Team1_Player1Id { get; set; }

    [ForeignKey("Team1_Player1Id")]
    public Member? Team1_Player1 { get; set; }

    public int? Team1_Player2Id { get; set; }

    [ForeignKey("Team1_Player2Id")]
    public Member? Team1_Player2 { get; set; }

    // Team 2
    [Required]
    public int Team2_Player1Id { get; set; }

    [ForeignKey("Team2_Player1Id")]
    public Member? Team2_Player1 { get; set; }

    public int? Team2_Player2Id { get; set; }

    [ForeignKey("Team2_Player2Id")]
    public Member? Team2_Player2 { get; set; }

    // Result
    public WinningSide WinningSide { get; set; } = WinningSide.None;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();
}
