using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Participants")]
public class Participant
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ChallengeId { get; set; }

    [ForeignKey("ChallengeId")]
    public Challenge? Challenge { get; set; }

    [Required]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    public Member? Member { get; set; }

    public ParticipantTeam Team { get; set; } = ParticipantTeam.None;

    public bool EntryFeePaid { get; set; } = false;

    [Column(TypeName = "decimal(18,2)")]
    public decimal EntryFeeAmount { get; set; } = 0;

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    public ParticipantStatus Status { get; set; } = ParticipantStatus.Pending;
}
