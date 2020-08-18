using System;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce.Api;
using Ucommerce.Search.Models;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class AddToBasketButtonController : BaseController
    {
        private readonly ITransactionLibrary _transactionLibrary;
        private readonly ICatalogContext _catalogContext;
	    private readonly IMiniBasketService _miniBasketService;

	    public AddToBasketButtonController(ITransactionLibrary transactionLibrary, ICatalogContext catalogContext, IMiniBasketService miniBasketService)
        {
            _transactionLibrary = transactionLibrary;
            _catalogContext = catalogContext;
		    _miniBasketService = miniBasketService;
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
                IsProductFamily = product.ProductType == ProductType.ProductFamily
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToBasket(AddToBasketButtonAddToBasketViewModel viewModel)
        {
            _transactionLibrary.AddToBasket(viewModel.Quantity, viewModel.ProductSku, viewModel.VariantSku);

	        return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
        }
    }
}