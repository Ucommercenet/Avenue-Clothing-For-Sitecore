using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using UCommerce.EntitiesV2;

namespace AvenueClothing.Feature.Catalog.Controllers
{
	public class SearchController : BaseController
    {

        public ActionResult Rendering()
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            CategoryRenderingViewModel productsViewModel = new CategoryRenderingViewModel();
            List<Guid> guids = new List<Guid>();

            if (!string.IsNullOrEmpty(keyword))
            {
                products = Product.Find(p =>
                                      p.VariantSku == null
                                      && p.DisplayOnSite
                                      &&
                                          (
                                          p.Sku.Contains(keyword)
                                          || p.Name.Contains(keyword)
                                          || p.ProductDescriptions.Any(d => d.DisplayName.Contains(keyword)
                                          || d.ShortDescription.Contains(keyword)
                                          || d.LongDescription.Contains(keyword)
                                          )
                                      )
                                  );
            }

            productsViewModel.ProductItemGuids = guids;
            foreach (var product in products.Where(x => x.DisplayOnSite))
            {
                productsViewModel.ProductItemGuids.Add(product.Guid);
            }

            
            return View(productsViewModel);
        }
    }
}