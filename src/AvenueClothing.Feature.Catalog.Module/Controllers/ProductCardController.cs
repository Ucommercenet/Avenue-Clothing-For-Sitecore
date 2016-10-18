using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Search.Facets;
using UCommerce.Search;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class ProductCardController : Controller
	{
		public ActionResult ProductCard()
		{
			var productView = new ProductViewModel();
			
			Database database = Sitecore.Context.Database;
			Item productItem = database.GetItem(RenderingContext.Current.Rendering.Properties["productItem"]);
			
			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			var currentProduct = productRepository.SingleOrDefault(x => x.Guid == productItem.ID.Guid);
			productView.Url = CatalogLibrary.GetNiceUrlForProduct(currentProduct, SiteContext.Current.CatalogContext.CurrentCategory);

			productView.PriceCalculation = CatalogLibrary.CalculatePrice(currentProduct);
			
			return View("/views/ProductCard.cshtml", productView);
		}
	}
}
