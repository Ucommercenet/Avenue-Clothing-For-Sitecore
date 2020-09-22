using System;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.UserFeedback.ViewModels;
using Sitecore.Mvc.Presentation;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace AvenueClothing.Project.UserFeedback.Controllers
{
	public class ReviewFormController : BaseController
    {
	    private readonly ICatalogContext _catalogContext;
	    private readonly IRepository<Product> _productRepository;
	    private readonly IRepository<ProductReviewStatus> _productReviewStatusRepository;
	    private readonly IOrderContext _orderContext;
	    private readonly IPipeline<ProductReview> _productReviewPipeline;

	    public ReviewFormController(ICatalogContext catalogContext, IRepository<Product> productRepository, IRepository<ProductReviewStatus> productReviewStatusRepository,
			IOrderContext orderContext, IPipeline<ProductReview> productReviewPipeline )
	    {
		    _catalogContext = catalogContext;
		    _productRepository = productRepository;
			_productReviewStatusRepository = productReviewStatusRepository;
		    _orderContext = orderContext;
		    _productReviewPipeline = productReviewPipeline;
	    }

	    public ActionResult Rendering()
        {
            var viewModel = new ReviewFormRenderingViewModel()
            {
                ProductGuid = RenderingContext.Current.ContextItem.ID.Guid,
            };
			if (_catalogContext.CurrentCategory != null)
            {
                viewModel.CategoryGuid = _catalogContext.CurrentCategory.Guid;
            }

	        viewModel.SubmitReviewUrl = Url.Action("SaveReview");
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveReview(ReviewFormSaveReviewViewModel viewModel)
        {
            var product = _productRepository.SingleOrDefault(x => x.Guid.ToString() == viewModel.ProductGuid);

            var catalogGroup = _catalogContext.CurrentCatalogGroup;
            var catalogGroupV2 = ProductCatalogGroup.FirstOrDefault(x => x.Guid == catalogGroup.Guid);

            var request = System.Web.HttpContext.Current.Request;
            var basket = _orderContext.GetBasket();

            var name = viewModel.Name;
            var email = viewModel.Email;
            var rating = viewModel.Rating * 20;
            var reviewHeadline = viewModel.Title;
            var reviewText = viewModel.Comments;

            if (basket.PurchaseOrder.Customer == null)
            {
                basket.PurchaseOrder.Customer = new Customer()
                {
                    FirstName = name,
                    LastName = String.Empty,
                    EmailAddress = email
                };
            }
            else
            {
                basket.PurchaseOrder.Customer.FirstName = name;
                if (basket.PurchaseOrder.Customer.LastName == null)
                {
                    basket.PurchaseOrder.Customer.LastName = String.Empty;
                }
                basket.PurchaseOrder.Customer.EmailAddress = email;
            }

            basket.PurchaseOrder.Customer.Save();

            var review = new ProductReview();
            review.ProductCatalogGroup = catalogGroupV2;
            review.ProductReviewStatus = _productReviewStatusRepository.SingleOrDefault(s => s.Name == "New");
            review.CreatedOn = DateTime.Now;
            review.CreatedBy = "System";
            review.Product = product;
            review.Customer = basket.PurchaseOrder.Customer;
            review.Rating = rating;
            review.ReviewHeadline = reviewHeadline;
            review.ReviewText = reviewText;
            review.Ip = request.UserHostName;

            product.AddProductReview(review);

            _productReviewPipeline.Execute(review);


            return Json(new {Rating = review.Rating, ReviewHeadline = review.ReviewHeadline, CreatedBy = review.CreatedBy, CreatedOn = review.CreatedOn.ToString("MMM dd, yyyy"), CreatedOnForMeta=review.CreatedOn.ToString("yyyy-MM-dd"), Comments = review.ReviewText }, JsonRequestBehavior.AllowGet);
        }
    }
}