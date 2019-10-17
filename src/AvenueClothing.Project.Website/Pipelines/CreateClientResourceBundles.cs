using System.Web.Optimization;
using Sitecore.Pipelines;

namespace AvenueClothing.Project.Website.Pipelines
{
    public class CreateClientResourceBundles : Sitecore.Mvc.Pipelines.Loader.InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            RegisterBundles(BundleTable.Bundles);
        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/require").Include(
                "~/Scripts/require-2.3.2.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jsComponents").Include(
                "~/Scripts/js*"));

            bundles.Add(new StyleBundle("~/styles/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Css/font-awesome.min.css",
                "~/Css/uCommerce.demostore.css"
            ));
        }
    }
}