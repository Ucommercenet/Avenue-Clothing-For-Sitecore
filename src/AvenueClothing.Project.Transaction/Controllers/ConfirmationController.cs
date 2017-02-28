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
using UCommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class ConfirmationController : BaseController
	{
	    private readonly TransactionLibraryInternal _transactionLibraryInternal;
	    private readonly CatalogLibraryInternal _catalogLibraryInternal;

	    public ConfirmationController(TransactionLibraryInternal transactionLibraryInternal, CatalogLibraryInternal catalogLibraryInternal)
	    {
            _transactionLibraryInternal = transactionLibraryInternal;
	        _catalogLibraryInternal = catalogLibraryInternal;
	    }
		public ActionResult Rendering()
		{
            var confirmation= new ConfirmationViewModel();
            confirmation.Headline = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Headline"));
		    confirmation.Message = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Message"));
		    confirmation.FirstName = Tracker.Current.Session.CustomData["FirstName"].ToString();

		    var purchaseOrderGuid = Request.QueryString["orderGuid"];
		    var purchaseOrder = PurchaseOrder.FirstOrDefault(x => x.OrderGuid.ToString() == purchaseOrderGuid);

		    var firstProduct = _catalogLibraryInternal.GetProduct(purchaseOrder.OrderLines.First().Sku);

		    confirmation.FirstOrderProductName = firstProduct.Name;
		    confirmation.FirstOrderProductImage =
		        ObjectFactory.Instance.Resolve<IImageService>().GetImage(firstProduct.PrimaryImageMediaId).Url;
            return View(confirmation);
		}
	}
}