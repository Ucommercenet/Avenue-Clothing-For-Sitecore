using System;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using UCommerce.Runtime;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
    public class AddToBasketController : Controller
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly ICatalogContext _catalogContext;

        public AddToBasketController(TransactionLibraryInternal transactionLibraryInternal, ICatalogContext catalogContext)
        {
            _transactionLibraryInternal = transactionLibraryInternal;
            _catalogContext = catalogContext;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var product = _catalogContext.CurrentProduct;

            var viewModel = new AddToBasketIndexViewModel
            {
                AddToBasketUrl = Url.Action("AddToBasket"),
                BasketUrl = "/cart",
                ConfirmationMessageTimeoutInMillisecs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                ConfirmationMessageClientId = "js-add-to-basket-button-confirmation-message-" + Guid.NewGuid(),
                ProductSku = product.Sku,
                ProductIsVariant = product.IsVariant
            };
            return View(viewModel);
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

            _transactionLibraryInternal.AddToBasket(viewModel.Quantity, viewModel.ProductSku, viewModel.VariantSku);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}