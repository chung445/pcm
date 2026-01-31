using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_MatchScores")]
public class MatchScore
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    [ForeignKey("MatchId")]
    public Match? Match { get; set; }

    public int SetNumber { get; set; }

    public int Team1Score { get; set; }

    public int Team2Score { get; set; }

    public bool IsFinalSet { get; set; } = false;
}
