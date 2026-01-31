using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_Transactions")]
public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public TransactionCategory? Category { get; set; }

    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public Member? Creator { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Approval/status
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedDate { get; set; }
}
