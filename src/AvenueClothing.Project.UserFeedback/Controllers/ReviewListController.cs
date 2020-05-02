using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.UserFeedback.ViewModels;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;

namespace AvenueClothing.Project.UserFeedback.Controllers
{
	public class ReviewListController : BaseController
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
	        var currentProductV2 = Product.FirstOrDefault(x => x.Sku == currentProduct.Sku);

            viewModel.Reviews = currentProductV2.ProductReviews.Select(review => new ReviewListRenderingViewModel.Review
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