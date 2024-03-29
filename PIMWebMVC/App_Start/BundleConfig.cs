﻿using System.Web;
using System.Web.Optimization;

namespace PIMWebMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jQuery/jquery-{version}.js",
                        "~/Scripts/jQuery/jquery-ui.js",
                        "~/Scripts/jQuery/jquery-ui-i18n.min.js",
                         "~/Scripts/jQuery/jquery-ui-i18n.extend.js",
                        "~/Scripts/Notify/notify.min.js",
                        "~/Scripts/ultility-helper.js",
                        "~/Scripts/global.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/projectSearch").Include(
                      "~/Scripts/Projects/project-model.js",
                      "~/Scripts/Projects/project-search-component.js"));

            bundles.Add(new ScriptBundle("~/bundles/project").Include(
                      "~/Scripts/Projects/project-component.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/site.css",
                      "~/Content/jquery-date-picker-override.css",
                      "~/Content/project-search.css"));
        }
    }
}
