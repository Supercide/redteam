using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
  public class SearchController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    //
    // GET: /Search
    [ValidateInput(false)]
    public ActionResult Index(string searchTerm)
    {
      // Reading from query string to demonstrate HTTP verb tampering
      var searchTermQuery = Request.QueryString["searchTerm"];
      if (!string.IsNullOrEmpty(searchTermQuery) && searchTermQuery.Contains("<"))
      {
        return View("IllegalChar");
      }

      var supercars = db.Supercars
                        .Where(s =>
                               s.Make.Name.Contains(searchTerm)
                               || s.Model.Contains(searchTerm)
                               || s.Description.Contains(searchTerm))
                        .Include("Make")
                        .Select(s => new Search
                        {
                          SupercarId = s.SupercarId,
                          Make = s.Make.Name,
                          Model = s.Model,
                          PowerKw = s.PowerKw,
                          TorqueNm = s.TorqueNm,
                          ZeroToOneHundredKmInSecs = s.ZeroToOneHundredKmInSecs,
                          TopSpeedKm = s.TopSpeedKm
                        });

      ViewBag.RawSearchTerm = searchTerm;

      if (!string.IsNullOrEmpty(Request.QueryString["searchTerm"]))
      {
        searchTerm = searchTerm.Replace("<", "").Replace(">", "").Replace("/", "&#47;");
      }

      ViewBag.SearchTerm = searchTerm;
      ViewBag.EncodedSearchTerm = searchTerm;

      return View(supercars);
    }
  }
}
