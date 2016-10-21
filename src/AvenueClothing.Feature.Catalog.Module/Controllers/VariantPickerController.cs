using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class VariantPickerController : Controller
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
	    private readonly ICatalogContext _catalogContext;

	    public VariantPickerController(IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> getProductPipeline, ICatalogContext catalogContext)
	    {
		    _getProductPipeline = getProductPipeline;
		    _catalogContext = catalogContext;
	    }

	    public ActionResult Index()
        {
			var currentProduct = _catalogContext.CurrentProduct;

            var viewModel = new VariantPickerViewModel
            {
                ProductSku = currentProduct.Sku,
                VariantExistsUrl = Url.Action("VariantExists")
            };

            if (!currentProduct.ProductDefinition.IsProductFamily())
            {
                return View(viewModel);
            }

            var uniqueVariants = currentProduct.Variants.SelectMany(p => p.ProductProperties)
                .Where(v => v.ProductDefinitionField.DisplayOnSite)
                .GroupBy(v => v.ProductDefinitionField)
                .Select(g => g);

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
            var getProductResponse = new GetProductResponse();
            if (_getProductPipeline.Execute(new GetProductPipelineArgs(new GetProductRequest(new ProductIdentifier(viewModel.ProductSku, null)), getProductResponse)) == PipelineExecutionResult.Error)
            {
                return Json(new { ProductVariantSku = "" });
            }

            var product = getProductResponse.Product;

            if (!product.ProductDefinition.IsProductFamily())
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