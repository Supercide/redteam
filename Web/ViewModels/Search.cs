using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
  public class Search
  {
    public int SupercarId { get; set; }

    [Display(Name = "Make")]
    public string Make { get; set; }

    [Display(Name = "Model")]
    public string Model { get; set; }

    [Display(Name = "Power")]
    [DisplayFormat(DataFormatString = "{0:n0}")]
    public int PowerKw { get; set; }

    [Display(Name = "Torque")]
    [DisplayFormat(DataFormatString = "{0:n0}")]
    public int TorqueNm { get; set; }

    [DisplayFormat(DataFormatString = "{0}s")]
    public double ZeroToOneHundredKmInSecs { get; set; }

    [Display(Name = "Top speed")]
    public int TopSpeedKm { get; set; }
  }
}