using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
  public class Vote
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VoteId { get; set; }

    public string Comments { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile User { get; set; }
    public int UserId { get; set; }

    [ForeignKey("SupercarId")]
    public virtual Supercar Supercar { get; set; }
    public int SupercarId { get; set; }
  }
}