using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce.Api;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.GetProduct;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class VariantPickerController : BaseController
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogLibrary _catalogLibrary;
        private readonly IIndex<Ucommerce.Search.Models.Product> _productIndex;

        public VariantPickerController(
            IPipeline<IPipelineArgs<GetProductRequest,
            GetProductResponse>> getProductPipeline,
            ICatalogContext catalogContext,
            ICatalogLibrary catalogLibrary,
            IIndex<Ucommerce.Search.Models.Product> productIndex)
        {
            _getProductPipeline = getProductPipeline;
            _catalogContext = catalogContext;
            _catalogLibrary = catalogLibrary;
            _productIndex = productIndex;
        }

        public ActionResult Rendering()
        {
            var currentProduct = _catalogContext.CurrentProduct;

            var viewModel = new VariantPickerRenderingViewModel
            {
                ProductSku = currentProduct.Sku,
                VariantExistsUrl = Url.Action("VariantExists")
            };

            if (currentProduct.ProductType != ProductType.ProductFamily)
            {
                return View(viewModel);
            }

            var variants = _catalogLibrary.GetVariants(currentProduct);

            var uniqueVariantProperties =
                from v in variants.SelectMany(p => p.GetUserDefinedFields())
                group v by v.Key
                into g
                select g;

            foreach (var variantProperty in uniqueVariantProperties)
            {
                var productPropertiesViewModel = new VariantPickerRenderingViewModel.Variant
                {
                    Name =  variantProperty.Key,
                    DisplayName = variantProperty.Key
                };

                foreach (var value in variantProperty.Select(p => p.Value).Distinct())
                {
                    productPropertiesViewModel.VaraintItems.Add(new VariantPickerRenderingViewModel.Variant.VaraintValue{
                        Name = value.ToString(),
                        DisplayName = value.ToString()
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
            Ucommerce.Search.Models.Product product;
            try
            {
                product = _catalogLibrary.GetProduct(viewModel.ProductSku);
            }
            catch (Exception e)
            {
                return Json(new {ProductVariantSku = ""});
            }

            if (product.ProductType != ProductType.ProductFamily)
            {
                return Json(new { ProductVariantSku = "" });
            }
            if (!viewModel.VariantNameValueDictionary.Any())
            {
                return Json(new { ProductVariantSku = "" });
            }

            var variant =
                _catalogLibrary
                    .GetVariants(product)
                    .FirstOrDefault(v => v.GetUserDefinedFields()
                                        .All(variantProperty => viewModel.VariantNameValueDictionary
                                            .Any(kvp =>
                                                    kvp.Key.Equals(variantProperty.Key, StringComparison.InvariantCultureIgnoreCase)
                                                    && kvp.Value.Equals(variantProperty.Value.ToString(), StringComparison.InvariantCultureIgnoreCase))));

            var variantSku = variant != null ? variant.VariantSku : "";


            return Json(new { ProductVariantSku = variantSku });
        }

        [HttpPost]
        public ActionResult GetAvailableCombinations(VariantPickerVariantExistsViewModel viewModel)
        {
            var selectedDictionary = viewModel.VariantNameValueDictionary.Where(x => x.Value != "").ToList();

            var currentProduct = _catalogLibrary.GetProduct(viewModel.ProductSku);

            IList<ProductPropertiesViewModel> result = new List<ProductPropertiesViewModel>();
            IList<Ucommerce.Search.Models.Product> possibleVariants = new List<Ucommerce.Search.Models.Product>();

            foreach (var kvp in selectedDictionary)
            {
                var variants = _catalogLibrary.GetVariants(currentProduct);

                foreach (var v in variants)
                {
                    if (v.GetUserDefinedFields().Any(x =>
                        x.Key.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase)
                        && x.Value.ToString().Equals(kvp.Value, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        possibleVariants.Add(v);
                    }
                }

                foreach (var possibleVariant in possibleVariants)
                {
                    var properties = possibleVariant.GetUserDefinedFields()
                        .Where(property => !property.Key.ToString().Equals(kvp.Key.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                           && !property.Value.ToString().Equals(kvp.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));

                    foreach (var prop in properties)
                    {
                        ProductPropertiesViewModel property = new ProductPropertiesViewModel();
                        property.PropertyName = prop.Key;
                        property.Values.Add(prop.Value.ToString());

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