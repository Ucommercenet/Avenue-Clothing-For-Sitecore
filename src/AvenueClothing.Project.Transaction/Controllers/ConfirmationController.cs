using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore.Analytics;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using UCommerce.Catalog;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class ConfirmationController : BaseController
	{
	    private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
	    private readonly IRepository<Product> _productRepository;
	    private readonly IImageService _imageService;
	    public ConfirmationController(IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<Product> productRepository, IImageService imageService)
	    {
	        _purchaseOrderRepository = purchaseOrderRepository;
	        _productRepository = productRepository;
	        _imageService = imageService;
	    }
		public ActionResult Rendering()
        {
            var confirmation = new ConfirmationViewModel();
            confirmation.Headline = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Headline"));
            confirmation.Message = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Message"));
            confirmation.FirstName = Tracker.Current.Session.CustomData["FirstName"].ToString();

            PurchaseOrder purchaseOrder = GetCurrentPurchaseOrder();

            var firstOrderLine = purchaseOrder.OrderLines.FirstOrDefault();
            var firstProduct = _productRepository.SingleOrDefault(x => x.Sku == firstOrderLine.Sku && x.VariantSku == firstOrderLine.VariantSku);
            var primaryImageMediaId = firstProduct.PrimaryImageMediaId;
            if (string.IsNullOrEmpty(primaryImageMediaId) && firstProduct.IsVariant)
            {
                primaryImageMediaId = firstProduct.ParentProduct.PrimaryImageMediaId;
            }
            if (firstOrderLine != null)
            {
                confirmation.FirstOrderProductName = firstOrderLine.ProductName;
                confirmation.FirstOrderProductImage = _imageService.GetImage(primaryImageMediaId).Url;
            }

            // Related product
            MapRelatedProductInformation(confirmation, purchaseOrder);
            return View(confirmation);
        }

        private void MapRelatedProductInformation(ConfirmationViewModel confirmation, PurchaseOrder purchaseOrder)
        {
            foreach (var orderline in purchaseOrder.OrderLines)
            {
                var productWithRelation =
                    _productRepository.SingleOrDefault(x => x.Sku == orderline.Sku && x.ProductRelations.Count > 0);

                if (productWithRelation != null)
                {
                    var productRelation = productWithRelation.ProductRelations.FirstOrDefault();
                    if (productRelation != null)
                    {
                        confirmation.RelatedProductName = productRelation.RelatedProduct.Name;
                        confirmation.ProductWithRelationName = productWithRelation.Name;
                        confirmation.RelatedProductImageUrl = _imageService.GetImage(productRelation.RelatedProduct.PrimaryImageMediaId).Url;
                       
                    }
                    break;
                }
            }
        }

        private PurchaseOrder GetCurrentPurchaseOrder()
        {
            var purchaseOrderGuid = Request.QueryString["orderGuid"];
            var purchaseOrder = _purchaseOrderRepository.SingleOrDefault(x => x.OrderGuid.ToString() == purchaseOrderGuid);
            return purchaseOrder;
        }
    }
}