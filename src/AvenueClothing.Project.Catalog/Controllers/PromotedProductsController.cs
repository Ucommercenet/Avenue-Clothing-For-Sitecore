using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using UCommerce.Runtime;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class PromotedProductsController : SitecoreController
	{
	    private readonly ICatalogContext _catalogContext;

	    public PromotedProductsController(ICatalogContext catalogContext)
	    {
	        _catalogContext = catalogContext;
	    }

		public ActionResult Rendering()
		{
			var promotedProductsViewModel = new PromotedProductsViewModel();

		    promotedProductsViewModel.ProductGuids =
                _catalogContext.CurrentCatalog.Categories.SelectMany(
		            c =>
		                c.Products.Where(
		                    p =>
		                        p.ProductProperties.Any(
		                            pp => pp.ProductDefinitionField.Name == "ShowOnHomepage" && Convert.ToBoolean(pp.Value)))).Select(x=>x.Guid.ToString()).ToList();


            promotedProductsViewModel.ProductCardRendering = RenderingContext.Current.Rendering.DataSource;

			return View(promotedProductsViewModel);
		}
	}
}