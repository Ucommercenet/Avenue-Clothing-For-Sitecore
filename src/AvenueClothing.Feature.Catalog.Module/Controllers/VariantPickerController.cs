using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class VariantPickerController : Controller
    {
        public ActionResult Index()
        {
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            var uniqueVariants = currentProduct.Variants.SelectMany(p => p.ProductProperties)
                    .Where(v => v.ProductDefinitionField.DisplayOnSite)
                    .GroupBy(v => v.ProductDefinitionField)
                    .Select(g => g);

            var viewModel = new VariantPickerViewModel
            {
                ProductSku = currentProduct.Sku,
                VariantExistsUrl = Url.Action("VariantExists")
            };

            foreach (var variant in uniqueVariants)
            {
                var productPropertiesViewModel = new VariantPickerViewModel.Variant
                {
                    Name = variant.Key.Name,
                    DisplayName = variant.Key.Name
                };

                foreach (var variantValue in variant.Select(v => v.Value).Distinct())
                {
                    productPropertiesViewModel.VaraintItems.Add(new VariantPickerViewModel.Variant.VaraintValue
                    {
                        Name = variantValue,
                        DisplayName = variantValue
                    });
                }
                viewModel.Variants.Add(productPropertiesViewModel);
            }

            
            return View(viewModel);
        }

        /// <summary>
        /// POST /api/Sitecore/VariantPicker/VariantExists/
        /// </summary>
        [HttpPost]
        public ActionResult VariantExists(VariantExistsViewModel viewModel)
        {
            var product = SiteContext.Current.CatalogContext.CurrentProduct;

            if (!product.IsVariant)
            {
                return Json(new { ProductVariantSku = "" });
            }
            if (!viewModel.VariantNameValueDictionary.Any())
            {
                return Json(new { ProductVariantSku = "" });
            }

            var variant = product.Variants.FirstOrDefault(v => v.ProductProperties
                      .Where(pp => pp.ProductDefinitionField.DisplayOnSite)
                      .Where(pp => pp.ProductDefinitionField.IsVariantProperty)
                      .Where(pp => !pp.ProductDefinitionField.Deleted)
                      .All(p => viewModel.VariantNameValueDictionary
                            .Any(kv => kv.Key.Equals(p.ProductDefinitionField.Name, StringComparison.InvariantCultureIgnoreCase) && kv.Value.Equals(p.Value, StringComparison.InvariantCultureIgnoreCase)))
                      );
            var variantSku = variant != null ? variant.VariantSku : "";


            return Json(new { ProductVariantSku = variantSku });
        }
    }
}