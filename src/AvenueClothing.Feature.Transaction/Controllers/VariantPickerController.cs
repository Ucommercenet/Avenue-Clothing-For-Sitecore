using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore.Collections;
using Sitecore.Mvc.Extensions;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class VariantPickerController : BaseController
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ICatalogContext _catalogContext;
        private readonly CatalogLibraryInternal _catalogLibraryInternal;

        public VariantPickerController(IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> getProductPipeline, ICatalogContext catalogContext, CatalogLibraryInternal catalogLibraryInternal)
        {
            _getProductPipeline = getProductPipeline;
            _catalogContext = catalogContext;
            _catalogLibraryInternal = catalogLibraryInternal;
        }

        public ActionResult Rendering()
        {
            var currentProduct = _catalogContext.CurrentProduct;

            var viewModel = new VariantPickerRenderingViewModel
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
                var productPropertiesViewModel = new VariantPickerRenderingViewModel.Variant
                {
                    Name = variant.Key.Name,
                    DisplayName = variant.Key.Name
                };

                foreach (var variantValue in variant.Select(v => v.Value).Distinct())
                {
                    productPropertiesViewModel.VaraintItems.Add(new VariantPickerRenderingViewModel.Variant.VaraintValue
                    {
                        Name = variantValue,
                        DisplayName = variantValue
                    });
                }
                viewModel.Variants.Add(productPropertiesViewModel);
            }
            viewModel.GetAvailableCombinationsUrl = Url.Action("GetAvailableCombinations");


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VariantExists(VariantPickerVariantExistsViewModel viewModel)
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

        [HttpPost]
        public ActionResult GetAvailableCombinations(VariantPickerVariantExistsViewModel viewModel)
        {
            var selectedDictionary = viewModel.VariantNameValueDictionary.Where(x => x.Value != "").ToList();

            var currentProduct = _catalogLibraryInternal.GetProduct(viewModel.ProductSku);

            IList<ProductPropertiesViewModel> result = new List<ProductPropertiesViewModel>();
            IList<Product> possibleVariants = new List<Product>();

            foreach (var kvp in selectedDictionary)
            {

                var variants = currentProduct.Variants;
                foreach (var v in variants)
                {
                    if (v.ProductProperties.Any(x => x.ProductDefinitionField.Name == kvp.Key && x.Value == kvp.Value))
                    {
                        possibleVariants.Add(v);
                    }
                }

                foreach (var possibleVariant in possibleVariants)
                {
                    var properties = ProductProperty.All()
                        .Where(x => x.ProductDefinitionField.Name != kvp.Key && x.Value != kvp.Value && x.Product == possibleVariant).Distinct();
                    foreach (var prop in properties)
                    {
                        ProductPropertiesViewModel property = new ProductPropertiesViewModel();
                        property.PropertyName = prop.ProductDefinitionField.Name;
                        property.Values.Add(prop.Value);

                        result.Add(property);
                    }

                }

            }

            return Json(new { properties = result });
        }
    }

    public class ProductPropertiesViewModel
    {
        public ProductPropertiesViewModel()
        {
            Values = new List<string>();
        }
        public string PropertyName { get; set; }
        public IList<string> Values { get; set; }


    }
}