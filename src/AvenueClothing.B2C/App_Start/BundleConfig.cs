using System.Web.Optimization;

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
                        "~/Scripts/js-quantity-picker.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-components-old").Include(
                "~/Scripts/uCommerce.facets.js", 
                "~/Scripts/js-add-to-basket-button.js",
                "~/Scripts/js-variant-picker.js",
                "~/Scripts/js-mini-basket.js",
                "~/Scripts/js-infinite-scrolling.js",
                "~/Scripts/uCommerce.demostore.productpage.js",
                "~/Scripts/js-address.js",
                "~/Scripts/js-shipping-picker.js",
                "~/Scripts/js-price-calculation.js",
                "~/Scripts/js-payment-picker.js",
                "~/Scripts/js-voucher.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Css/font-awesome.min.css",
                      "~/Css/uCommerce.demostore.css"
                      ));
        }
    }
}