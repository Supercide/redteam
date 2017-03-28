using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Web.Filters;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
  [InitialiseDatabase]
  public class SupercarController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    public ActionResult Index(int id = 0)
    {
      var supercar = db.Supercars
                       .Include("Make")
                       .Include("Votes")
                       .Include("Votes.User")
                       .SingleOrDefault(s => s.SupercarId == id);

      if (supercar == null)
      {
        return HttpNotFound();
      }

      ViewBag.TotalVotes = (decimal)db.Votes.Count();

      if (User.Identity.IsAuthenticated)
      {
        var profile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);
        if (profile != null)
        {
          ViewBag.FirstName = profile.FirstName;
          ViewBag.LastName = profile.LastName;
        }
      }

      return View(supercar);
    }

    public ActionResult Leaderboard(string orderBy = "votes", bool asc = true)
    {
      try
      {
        var supercars = db.Supercars.SqlQuery("SELECT * FROM Supercar ORDER BY " + (orderBy == "votes" ? "SupercarId" : orderBy) +(asc ? " ASC" : " DESC")).ToList();

        if (orderBy == "votes")
        {
          supercars = supercars.OrderByDescending(s => s.Votes.Count()).ToList();
        }

        var leaderboard = supercars.Select(s => new Leaderboard
          {
            SupercarId = s.SupercarId,
            Make = s.Make.Name,
            Model = s.Model,
            PowerKw = s.PowerKw,
            TorqueNm = s.TorqueNm,
            ZeroToOneHundredKmInSecs = s.ZeroToOneHundredKmInSecs,
            TopSpeedKm = s.TopSpeedKm,
            Votes = s.Votes.Count()
          });

        return View(leaderboard);
      }
      catch (SqlException)
      {
        return View("LeaderBoardError");
      }
    }
  }
}
