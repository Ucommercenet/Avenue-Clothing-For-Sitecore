using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
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
			var product = RenderingContext.CurrentOrNull.Rendering.Properties["productId"];

			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			int productId;
			if (int.TryParse(product, out productId))
			{
				var viewProduct = MapProduct(productRepository.Get(productId));
				return View("/views/ProductCard.cshtml", viewProduct);
			}
			return null;
		}

		private ProductViewModel MapProduct(Product product)
		{
			return new ProductViewModel()
			{
				Sku = product.Sku,
				Name = product.Name,
				Id = product.Id,
			};
		}
	}
}
