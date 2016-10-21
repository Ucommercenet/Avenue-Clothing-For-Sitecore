using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class QuantityPickerController : Controller
    {
	    private readonly ICatalogContext _catalogContext;

	    public QuantityPickerController(ICatalogContext catalogContext)
	    {
		    _catalogContext = catalogContext;
	    }

	    public ActionResult Index()
        {
			var currentProduct = _catalogContext.CurrentProduct;

            var viewModel = new QuantityPickerViewModel
            {
                ProductSku = currentProduct.Sku,
                MaxNumberOfItems = 30
            };
            return View(viewModel);
        }
    }
}