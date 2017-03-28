using System.Web.Optimization;

namespace Web
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                  "~/Scripts/jquery-ui-{version}.js"));   
      
      bundles.Add(new ScriptBundle("~/bundles/loginscripts").Include(
                  "~/Scripts/supercarScript.js",
                  "~/Scripts/jquery.cookie.js",
                  "~/Scripts/jquery.base64.js"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap-carousel").Include(
                  "~/Scripts/bootstrap-carousel.js"));  

      bundles.Add(new ScriptBundle("~/bundles/bootstrap-modal").Include(
                  "~/Scripts/bootstrap-modal.js"));
         
      bundles.Add(new ScriptBundle("~/bundles/vote").Include(
                  "~/Scripts/supercarScript.js",
                  "~/Scripts/vote.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.unobtrusive*",
                  "~/Scripts/jquery.validate*"));

      bundles.Add(new StyleBundle("~/Content/site").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/bootstrap-responsive.css",
                  "~/Content/site.css"));

    }
  }
}