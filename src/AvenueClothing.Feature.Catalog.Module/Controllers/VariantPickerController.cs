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
                ProductSku = currentProduct.Sku
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
    }
}