using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class ReviewDisplayController: Controller
    {
        public ActionResult ReviewDisplay()
        {
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;
            IList<ProductReviewViewModel> reviews = new List<ProductReviewViewModel>();
            foreach (var review in currentProduct.ProductReviews)
            {
                reviews.Add(new ProductReviewViewModel()
                {
                    Name = review.Customer.FirstName + " " + review.Customer.LastName,
                    Email = review.Customer.EmailAddress,
                    Title = review.ReviewHeadline,
                    CreatedOn = review.CreatedOn,
                    Comments = review.ReviewText,
                    Rating = review.Rating
                });
            }

            return View("~/Views/ReviewDisplay.cshtml", reviews);
        }
    }
}