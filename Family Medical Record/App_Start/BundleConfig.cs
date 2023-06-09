﻿using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Optimization;

namespace Family_Medical_Record
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                        "~/Scripts/jquery-1.11.1.min.js",
                        "~/Scripts/modernizr.custom.js",
                        "~/Scripts/wow.min.js",
                        "~/Scripts/metisMenu.min.js",
                        "~/Scripts/custom.js"
                ));

            bundles.Add(new Bundle("~/bundles/footer").Include(
                        "~/Scripts/classie.js",
                        "~/Scripts/jquery.nicescroll.js",
                        "~/Scripts/scripts.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/MyJs.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/style.css",
                      "~/Content/font-awesome.css",
                      "~/Content/animate.css",
                      "~/Content/custom.css",
                      "~/Content/site.css"));
        }
    }
}
