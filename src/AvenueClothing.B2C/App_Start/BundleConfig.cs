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

            //Change to wildcard when all js- files are changed to require.js
            //bundles.Add(new ScriptBundle("~/bundles/jsComponents").Include(
            //          "~/Scripts/js*.js"));
            bundles.Add(new ScriptBundle("~/bundles/jsComponents").Include(
                        "~/Scripts/jsQuantityPicker.js", 
                        "~/Scripts/jsAddToBasketButton.js",
                        "~/Scripts/jsVariantPicker.js",
                        "~/Scripts/jsAddress.js"));

            //Remove when all are converted to require.js
            bundles.Add(new ScriptBundle("~/bundles/js-components-old").Include(
                "~/Scripts/uCommerce.facets.js", 
                "~/Scripts/js-add-to-basket-button.js",
                "~/Scripts/jsVariantPicker.js",
                "~/Scripts/js-mini-basket.js",
                "~/Scripts/js-infinite-scrolling.js",
                "~/Scripts/uCommerce.demostore.productpage.js",
                "~/Scripts/jsAddress.js",
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