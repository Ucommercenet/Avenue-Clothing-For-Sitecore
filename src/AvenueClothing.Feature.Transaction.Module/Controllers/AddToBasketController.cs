using System;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using UCommerce.Runtime;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
    public class AddToBasketButtonController : Controller
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly ICatalogContext _catalogContext;

        public AddToBasketButtonController(TransactionLibraryInternal transactionLibraryInternal, ICatalogContext catalogContext)
        {
            _transactionLibraryInternal = transactionLibraryInternal;
            _catalogContext = catalogContext;
        }

        [HttpGet]
        public ActionResult Rendering()
        {
            var product = _catalogContext.CurrentProduct;

            var viewModel = new AddToBasketButtonRenderingViewModel
            {
                AddToBasketUrl = Url.Action("AddToBasket"),
                BasketUrl = "/cart",
                ConfirmationMessageTimeoutInMillisecs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                ConfirmationMessageClientId = "js-add-to-basket-button-confirmation-message-" + Guid.NewGuid(),
                ProductSku = product.Sku,
                IsProductFamily = product.ProductDefinition.IsProductFamily()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToBasket(AddToBasketButtonAddToBasketViewModel viewModel)
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