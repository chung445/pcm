using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.API.Models;

[Table("999_News")]
public class News
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }
}
