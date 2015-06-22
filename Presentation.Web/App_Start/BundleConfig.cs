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
                .Include("~/Scripts/modernizr-*")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-ui-router.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap-tpls.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/moment-locale-da.js")
                .Include("~/Scripts/moment-timezone-with-data.js")
                .Include("~/Scripts/angular-moment.js")
                .Include("~/Scripts/angular-resource.js")
                .Include("~/Scripts/kendo-ie-fix.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/respond.js")
                .Include("~/Scripts/kendo/kendo.all.min.js")
                .Include("~/Scripts/kendo/cultures/kendo.culture.da-DK.min.js")
                .Include("~/Scripts/loading-bar.min.js")
                .Include("~/Scripts/pnotify.custom.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/loading-bar.min.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/Themes/SbAdmin/css/bootstrap.min.css",
                      "~/Content/Themes/SbAdmin/css/sb-admin.css",
                      "~/Content/Themes/SbAdmin/font-awesome/css/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/timeline.css",
                      "~/Content/kendo/kendo.common.min.css",
                      "~/Content/kendo/kendo.common-bootstrap.min.css",
                      "~/Content/kendo/kendo.rtl.css",
                      "~/Content/kendo/kendo.bootstrap.min.css",
                      "~/Content/kendo/kendo.dataviz.min.css",
                      "~/Content/kendo/kendo.dataviz.bootstrap.min.css",
                      "~/Content/pnotify.custom.min.css",
                      "~/Content/custom.css"));


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
