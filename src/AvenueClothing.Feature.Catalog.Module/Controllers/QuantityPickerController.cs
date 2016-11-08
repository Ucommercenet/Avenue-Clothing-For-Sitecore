using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using AvenueClothing.Foundation.MvcExtensionsModule;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class QuantityPickerController : BaseController
    {
	    private readonly ICatalogContext _catalogContext;

	    public QuantityPickerController(ICatalogContext catalogContext)
	    {
		    _catalogContext = catalogContext;
	    }

	    public ActionResult Rendering()
        {
			var currentProduct = _catalogContext.CurrentProduct;

            var viewModel = new QuantityPickerRenderingViewModel
            {
                ProductSku = currentProduct.Sku,
                MaxNumberOfItems = 30
            };
            return View(viewModel);
        }
    }
}