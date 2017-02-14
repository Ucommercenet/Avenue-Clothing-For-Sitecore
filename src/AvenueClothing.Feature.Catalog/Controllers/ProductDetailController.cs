using System;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class ProductDetailController : SitecoreController
	{
		public ActionResult Rendering()
		{
			var productDetailViewModel = new ProductDetailViewModel();

            productDetailViewModel.LongDescription = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Long description"));
            productDetailViewModel.ReviewListRendering = RenderingContext.Current.Rendering.DataSource.Split('|')[0];
            productDetailViewModel.ReviewFormRendering = RenderingContext.Current.Rendering.DataSource.Split('|')[1];

            productDetailViewModel.ProductDetailsDetails = "tab-details-" + Guid.NewGuid();
            productDetailViewModel.ProductDetailsDelivery = "tab-delivery-" + Guid.NewGuid();
            productDetailViewModel.ProductDetailsReturns = "tab-returns-" + Guid.NewGuid();
            productDetailViewModel.ProductDetailsReviews = "tab-reviews-" + Guid.NewGuid();


            return View(productDetailViewModel);
		}
	}
}
