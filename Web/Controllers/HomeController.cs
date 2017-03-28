using System.Linq;
using System.Web.Mvc;
using Web.Filters;
using Web.ViewModels;

namespace Web.Controllers
{
  [InitialiseDatabase]
  public class HomeController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    //
    // GET: /
    public ActionResult Index()
    {
      var homePage = new HomePage
        {
          TopCars = db.Supercars
                      .OrderByDescending(s => s.Votes.Count())
                      .Take(3)
                      .Select(s => new TopCar
                      {
                        SupercarId = s.SupercarId,
                        Make = s.Make.Name,
                        Model = s.Model,
                        Description = s.Description
                      }),

          TopMakes = db.Votes
                       .GroupBy(v => v.Supercar.MakeId)
                       .OrderByDescending(v => v.Count())
                       .Take(3)
                       .Join(db.Makes, c => c.Key, cm => cm.MakeId, (c, cm) => new TopMake
                         {
                           MakeId = c.Key,
                           Name = cm.Name,
                           Votes = c.Count(),
                           Supercars = cm.Supercars.Count()
                         })
          };

      return View(homePage);
    }
  }
}
