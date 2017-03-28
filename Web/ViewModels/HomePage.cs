using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
  public class HomePage
  {
    public IEnumerable<TopCar> TopCars { get; set; }

    public IEnumerable<TopMake> TopMakes { get; set; }
  }

  public class TopCar
  {
    public int SupercarId { get; set; }

    public string Make { get; set; }

    public string Model { get; set; }

    public string Description { get; set; }
  }

  public class TopMake
  {
    public int MakeId { get; set; }

    public string Name { get; set; }

    [DisplayFormat(DataFormatString = "{0:n0}")]
    public int Votes { get; set; }

    [DisplayFormat(DataFormatString = "{0:n0}")]
    public int Supercars { get; set; }
  }
}