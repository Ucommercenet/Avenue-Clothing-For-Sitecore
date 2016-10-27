using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Mvc.Controllers;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class ReviewListController: SitecoreController
    {
	    private readonly ICatalogContext _catalogContext;

	    public ReviewListController(ICatalogContext catalogContext)
	    {
		    _catalogContext = catalogContext;
	    }

	    public ActionResult Rendering()
	    {
	        var viewModel = new ReviewListRenderingViewModel();

            var currentProduct = _catalogContext.CurrentProduct;

            viewModel.Reviews = currentProduct.ProductReviews.Select(review => new ReviewListRenderingViewModel.Review
            {
	            Name = review.Customer.FirstName + " " + review.Customer.LastName,
	            Email = review.Customer.EmailAddress,
	            Title = review.ReviewHeadline,
	            CreatedOn = review.CreatedOn,
	            Comments = review.ReviewText,
	            Rating = review.Rating
	        }).ToList();

            return View(viewModel);
        }
    }
}