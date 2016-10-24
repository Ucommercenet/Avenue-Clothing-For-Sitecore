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

	    public ActionResult Rendering()
        {
            var products = CatalogLibrary.GetCatalog().Categories.SelectMany(c=>c.Products.Where(p => p.ProductProperties.Any(pp => pp.ProductDefinitionField.Name == "ShowOnHomepage" && Convert.ToBoolean(pp.Value))));
            HomepageCatalogViewModel viewModel = new HomepageCatalogViewModel();

            foreach (var p in products)
            {
                viewModel.Products.Add(new ProductCardRenderingViewModel()
                {
                    Url = CatalogLibrary.GetNiceUrlForProduct(p),
                    ProductSku = p.Sku,
                });
            }

            return View(viewModel);
        }
    }
}