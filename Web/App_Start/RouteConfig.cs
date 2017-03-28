using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute("ControllerId", "{controller}/{id}", new { action = "Index" }, new { id = @"\d+" });
      routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
    }
  }
}