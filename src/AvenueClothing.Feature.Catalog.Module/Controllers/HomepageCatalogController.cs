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
        public ActionResult HomepageCatalog()
        {
            var products = UCommerce.Api.CatalogLibrary.GetCatalog().Categories.SelectMany(c=>c.Products.Where(p => p.ProductProperties.Any(pp => pp.ProductDefinitionField.Name == "ShowOnHomepage" && Convert.ToBoolean(pp.Value))));
            HomepageCatalogViewModel homepageCatalogViewModel = new HomepageCatalogViewModel();

            foreach (var p in products)
            {
                homepageCatalogViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    Id = p.Id,
                    PriceCalculation = CatalogLibrary.CalculatePrice(p),
                    Url = CatalogLibrary.GetNiceUrlForProduct(p),
                    Sku = p.Sku,
                    IsVariant = p.IsVariant,
                    VariantSku = p.VariantSku,
                    ThumbnailImageUrl = ObjectFactory.Instance.Resolve<IImageService>().GetImage(p.ThumbnailImageMediaId).Url
                });
            }

            return View("~/Views/HomepageCatalog.cshtml", homepageCatalogViewModel);
        }
    }
}