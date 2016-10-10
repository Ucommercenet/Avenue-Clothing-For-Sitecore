using System.Net;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using UCommerce.Api;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
    public class ProductController : Controller
    {
        [HttpGet]
        public ActionResult AddToBasketButton()
        {
            var viewModel = new AddToBasketButtonViewModel
            {
                AddToBasketUrl = Url.Action("AddToBasket"),
                ValidateProductExistsUrl = Url.Action("ValidateProductExists")
            };
            return View(viewModel);
        }

        /// <summary>
        /// GET /api/Sitecore/Product/ValidateProductExists/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValidateProductExists(ValidateProductExistsViewModel viewModel)
        {
            //TODO: Check if product exists
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// POST /api/Sitecore/Product/AddToBasket/
        /// </summary>
        /// <param name="viewModel">Json or Http Form data</param>
        /// <returns>Http status codes</returns>
        [HttpPost]
        public ActionResult AddToBasket(AddToBasketViewModel viewModel)
        {
            if (viewModel.Quantity <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrEmpty(viewModel.ProductSku))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TransactionLibrary.AddToBasket(viewModel.Quantity, viewModel.ProductSku, viewModel.VariantSku);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}