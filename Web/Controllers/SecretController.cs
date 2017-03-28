using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
  public class SecretController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    //
    // GET: /Secret/Admin
    public ActionResult Admin()
    {
      if (User.Identity.IsAuthenticated && db.UserProfiles.Any(u => u.Email == User.Identity.Name && u.IsAdmin != null && u.IsAdmin.Value))
      {
        return View();
      }
      else
      {
        return View("AccessDenied");
      }
    }

    //
    // GET: /Secret/Users
    public ActionResult Users()
    {
      var users = db.UserProfiles;
      return View(users);
    }
  }
}
