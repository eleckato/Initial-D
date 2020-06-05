using System.Web;
using System.Web.Optimization;

namespace initial_d
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Good old Jquery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Jquery dependan library for form validation, it's used for ASP.NET build in validation
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/moment.js"
                        ));

            // This comment was auto generated
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bootstrap JS
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.js",
                      "~/Scripts/bootstrap.js"));

            // Sitewide Styles
            bundles.Add(new StyleBundle("~/ContentCSS").Include(
                      "~/Content/Bootstrap/bootstrap.min.css",  // Base Bootstrap styles
                      "~/Content/FontAwesome/fontawesome.min.css",  // Base FontAwesome icons
                      "~/Content/FontAwesome/solid.min.css",  // Solid FontAwesome icons
                      "~/Content/Site.css", // Sitewide styles
                      "~/Content/CSS/Elements/buttons.css",  // Custom buttons
                      "~/Content/CSS/Elements/c-table.css"  // Custom tables
                      ));

            // Sitewide Scripts
            bundles.Add(new ScriptBundle("~/MainScripts").Include(
                        "~/Scripts/js.cookie.min.js",   // Library to deal with cookies easyly from JS
                        "~/Content/JS/common.js",   // Common utilities and functions
                        "~/Content/JS/_layout.js"   // JS for _Layout.cshtml
                        ));

            bundles.Add(new ScriptBundle("~/HeadScrips").Include(
                    "~/Content/JS/head.util.js"
                ));

        }
    }
}
