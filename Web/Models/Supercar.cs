using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
  public class Supercar
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SupercarId { get; set; }

    [ForeignKey("MakeId")]
    public virtual Make Make { get; set; }
    public int MakeId { get; set; }

    [Required]
    [Display(Name = "Model")]
    [StringLength(100)]
    public string Model { get; set; }

    [Required]
    [Display(Name = "Description")]
    [StringLength(200)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Power")]
    [DisplayFormat(DataFormatString = "{0:n0}kw")]
    public int PowerKw { get; set; }

    [Required]
    [Display(Name = "Torque")]
    [DisplayFormat(DataFormatString = "{0:n0}Nm")]
    public int TorqueNm { get; set; }

    [Required]
    [Display(Name = "Weight")]
    [DisplayFormat(DataFormatString = "{0:n0}kg")]
    public int WeightKg { get; set; }

    [Required]
    [Display(Name = "0-100 kph")]
    [DisplayFormat(DataFormatString = "{0}s")]
    public double ZeroToOneHundredKmInSecs { get; set; }

    [Required]
    [Display(Name = "Top speed")]
    [DisplayFormat(DataFormatString = "{0}kph")]
    public int TopSpeedKm { get; set; }

    [Required]
    [Display(Name = "Engine layout")]
    [StringLength(100)]
    public string EngineLayout { get; set; }

    [Required]
    [Display(Name = "Engine capacity")]
    [DisplayFormat(DataFormatString = "{0}cc")]
    public int EngineCc { get; set; }

    public virtual ICollection<Vote> Votes { get; set; }
  }
}