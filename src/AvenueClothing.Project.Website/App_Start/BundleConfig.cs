﻿using System.Web.Optimization;

namespace AvenueClothing.Project.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/require").Include(
                        "~/Scripts/require-{version}.js"));

           

            bundles.Add(new ScriptBundle("~/bundles/jsComponents").Include(
                "~/Scripts/js*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Css/font-awesome.min.css",
                      "~/Css/uCommerce.demostore.css"
                      ));
        }
    }
}