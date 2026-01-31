using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Notifications")]
public class Notification
{
    [Key]
    public int Id { get; set; }

    public int? MemberId { get; set; }

    [ForeignKey("MemberId")]
    public Member? Member { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.Info;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? LinkUrl { get; set; }
}
