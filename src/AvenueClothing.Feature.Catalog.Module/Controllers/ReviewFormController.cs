using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Mvc.Presentation;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class ReviewFormController: Controller
    {
	    private readonly ICatalogContext _catalogContext;
	    private readonly IRepository<Product> _productRepository;
	    private readonly IRepository<ProductReviewStatus> _productReviewStatusRepository;
	    private readonly IRepository<Category> _categoryRepository;
	    private readonly IOrderContext _orderContext;
	    private readonly IPipeline<ProductReview> _productReviewPipeline;

	    public ReviewFormController(ICatalogContext catalogContext, IRepository<Product> productRepository, IRepository<ProductReviewStatus> productReviewStatusRepository, 
			IRepository<Category> categoryRepository, IOrderContext orderContext, IPipeline<ProductReview> productReviewPipeline )
	    {
		    _catalogContext = catalogContext;
		    _productRepository = productRepository;
			_productReviewStatusRepository = productReviewStatusRepository;
			_categoryRepository = categoryRepository;
		    _orderContext = orderContext;
		    _productReviewPipeline = productReviewPipeline;
	    }

	    public ActionResult ReviewForm()
        {
            CategoryProductGuid guids = new CategoryProductGuid()
            {
                ProductGuid = RenderingContext.Current.ContextItem.ID.Guid,
            };
			if (_catalogContext.CurrentCategory != null)
            {
				guids.CategoryGuid = _catalogContext.CurrentCategory.Guid;
            }
          
            return View("~/Views/ReviewForm.cshtml", guids);
        }

        [HttpPost]
        public ActionResult PostReview(ProductReviewViewModel formReview)
        {
     
            var product = _productRepository.SingleOrDefault(x=> x.Guid == formReview.ProductGuid);
			var category = _categoryRepository.SingleOrDefault(x => x.Guid == formReview.CategoryGuid);

            var request = System.Web.HttpContext.Current.Request;
			var basket = _orderContext.GetBasket();

            if (request.Form.AllKeys.All(x => x != "review-product"))
            {
                return View();
            }

            var name = formReview.Name;
            var email = formReview.Email;
            var rating = Convert.ToInt32(formReview.Rating) * 20;
            var reviewHeadline = formReview.Title;
            var reviewText = formReview.Comments;

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
            review.ProductCatalogGroup = _catalogContext.CurrentCatalogGroup;
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

            if (category != null)
            {
                return Redirect(CatalogLibrary.GetNiceUrlForProduct(product, category));
            }
            else
            {
                return Redirect(CatalogLibrary.GetNiceUrlForProduct(product));
            }
        }
    }
}