using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
  public class MakeController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    //
    // GET: /Make/
    [ValidateInput(true)]
    public ActionResult Index(int id, string orderBy = "SupercarId")
    {
      var make = db.Makes.Single(m => m.MakeId == id);
      var supercars = db.Supercars.SqlQuery("SELECT * FROM Supercar WHERE MakeID = " + id + " ORDER BY " + orderBy);
      
      ViewBag.Make = make.Name;
      ViewBag.MakeId = make.MakeId;

      return View(supercars);
    }
  }
}
