using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
  public class Make
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MakeId { get; set; }

    [Required]
    [Display(Name = "Make")]
    [StringLength(100)]
    public string Name { get; set; }

    public virtual ICollection<Supercar> Supercars { get; set; }
  }
}