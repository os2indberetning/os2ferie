using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace OS2Indberetning
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/libraries")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-ui-router.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap-tpls.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/moment-timezone-with-data.js")
                .Include("~/Scripts/angular-moment.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/Themes/SbAdmin/css/bootstrap.min.css",
                      "~/Content/Themes/SbAdmin/css/sb-admin.css",
                      "~/Content/Themes/SbAdmin/css/plugins/morris.css",
                      "~/Content/Themes/SbAdmin/font-awesome/css/font-awesome.min.css",
                      "~/Content/site.css",
                      "~/Content/custom.css"));

            //bundles.Add(new ScriptBundle("~/bundles/angular").IncludeDirectory("~/App", "*.js", true));
            
            bundles.Add(new ScriptBundle("~/bundles/jasminespec").IncludeDirectory("~/App", "*.spec.js", true));

            bundles.Add(new ScriptBundle("~/bundles/angular").IncludeDirectoryWithExclusion("~/App", "*.js", true, "*.spec.js"));            
        }
    }

    public static class BundleExtentions
    {
        public static Bundle IncludeDirectoryWithExclusion(this ScriptBundle bundle, string directoryVirtualPath, string searchPattern, bool includeSubDirectories, string excludePattern)
        {
            var folderPath = HttpContext.Current.Server.MapPath(directoryVirtualPath);
            SearchOption searchOption = includeSubDirectories
                                            ? SearchOption.AllDirectories
                                            : SearchOption.TopDirectoryOnly;

            var foundFiles = Directory.GetFiles(folderPath, searchPattern, searchOption);

            var filesToExclude = Directory.GetFiles(folderPath, excludePattern, searchOption);

            foreach (var file in foundFiles)
            {
                if (!filesToExclude.Contains(file))
                {
                    bundle.Include(directoryVirtualPath + file.Replace(folderPath, string.Empty).Replace("\\", "/"));
                }
            }

            return bundle;
        }
    }
}
