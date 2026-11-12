using System.ComponentModel.DataAnnotations;

namespace RazorPagesYacht.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int YachtId { get; set; }                  // which yacht (foreign key)

    [Required, StringLength(450)]
    public string UserId { get; set; } = string.Empty; // who wrote it (Identity user id)

    [StringLength(128)]
    public string? ReviewerName { get; set; }          // copy of the email to display

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(2000)]
    public string? AdminReply { get; set; }        // reply text

    [StringLength(128)]
    public string? AdminReplyBy { get; set; }      // which admin replied

    public DateTime? AdminRepliedAt { get; set; }  // when

    public Yacht? Yacht { get; set; }                  // navigation property
}