using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Presentation;
using UCommerce.EntitiesV2;

namespace AvenueClothing.Project.Catalog.Controllers
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

			productsViewModel.ProductCardRendering = RenderingContext.Current.Rendering.DataSource;

            return View(productsViewModel);
        }
    }
}