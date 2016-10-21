using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.Infrastructure;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class HomepageCatalogController: Controller
    {
	    private readonly IImageService _imageService;

	    public HomepageCatalogController(IImageService imageService)
	    {
		    _imageService = imageService;
	    }

	    public ActionResult HomepageCatalog()
        {
            var products = CatalogLibrary.GetCatalog().Categories.SelectMany(c=>c.Products.Where(p => p.ProductProperties.Any(pp => pp.ProductDefinitionField.Name == "ShowOnHomepage" && Convert.ToBoolean(pp.Value))));
            HomepageCatalogViewModel homepageCatalogViewModel = new HomepageCatalogViewModel();

            foreach (var p in products)
            {
                homepageCatalogViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    Id = p.Id,
                    PriceCalculation = CatalogLibrary.CalculatePrice(p),
                    Url = CatalogLibrary.GetNiceUrlForProduct(p),
                    ProductSku = p.Sku,
                    IsVariant = p.IsVariant,
                    VariantSku = p.VariantSku,
					ThumbnailImageUrl = _imageService.GetImage(p.ThumbnailImageMediaId).Url
                });
            }

            return View("~/Views/HomepageCatalog.cshtml", homepageCatalogViewModel);
        }
    }
}