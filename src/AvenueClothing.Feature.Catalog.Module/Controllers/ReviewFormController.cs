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
        public ActionResult ReviewForm()
        {
            CategoryProductGuid guids = new CategoryProductGuid()
            {
                ProductGuid = RenderingContext.Current.ContextItem.ID.Guid,
                CategoryGuid = SiteContext.Current.CatalogContext.CurrentCategory.Guid
            };
            return View("~/Views/ReviewForm.cshtml", guids);
        }

        [HttpPost]
        public ActionResult PostReview(ProductReviewViewModel formReview)
        {
     
            var product = Product.FirstOrDefault(x=> x.Guid == formReview.ProductGuid);
            var category = Category.FirstOrDefault(x => x.Guid == formReview.CategoryGuid);

            var request = System.Web.HttpContext.Current.Request;
            var basket = SiteContext.Current.OrderContext.GetBasket();

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
            review.ProductCatalogGroup = SiteContext.Current.CatalogContext.CurrentCatalogGroup;
            review.ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New");
            review.CreatedOn = DateTime.Now;
            review.CreatedBy = "System";
            review.Product = product;
            review.Customer = basket.PurchaseOrder.Customer;
            review.Rating = rating;
            review.ReviewHeadline = reviewHeadline;
            review.ReviewText = reviewText;
            review.Ip = request.UserHostName;

            product.AddProductReview(review);

            PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

            return Redirect(CatalogLibrary.GetNiceUrlForProduct(product, category));
            
        }
    }
}