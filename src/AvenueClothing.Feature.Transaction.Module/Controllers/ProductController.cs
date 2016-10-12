using System;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
    public class ProductController : Controller
    {
        private readonly IPipelineTask<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ILoggingService _loggingService;

        public ProductController(IPipelineTask<IPipelineArgs<GetProductRequest, GetProductResponse>> getProductPipeline, ILoggingService loggingService)
        {
            _getProductPipeline = getProductPipeline;
            _loggingService = loggingService;
        }

        [HttpGet]
        public ActionResult AddToBasketButton()
        {
            var viewModel = new AddToBasketButtonViewModel
            {
                AddToBasketUrl = Url.Action("AddToBasket"),
                ValidateProductExistsUrl = Url.Action("ValidateProductExists"),
                BasketUrl = "",//TODO: Get the url from sitecore?
                ConfirmationMessageTimeoutInMillisecs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                ConfirmationMessageClientId = "js-add-to-basket-button-confirmation-message-" + Guid.NewGuid()
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
            var productIdentifier = new ProductIdentifier(viewModel.ProductSku, viewModel.VariantSku);
            var getProductResponse = new GetProductResponse();

            try
            {
                _getProductPipeline.Execute(new GetProductPipelineArgs(new GetProductRequest(productIdentifier), getProductResponse));
            }
            catch (ArgumentException ex)
            {
                _loggingService.Log<ProductController>(ex);

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
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