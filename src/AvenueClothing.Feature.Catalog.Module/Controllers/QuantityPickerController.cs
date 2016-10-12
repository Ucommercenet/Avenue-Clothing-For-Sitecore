using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class QuantityPickerController : Controller
    {
        public ActionResult Index()
        {
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            var viewModel = new QuantityPickerViewModel
            {
                ProductSku = currentProduct.Sku,
                MaxNumberOfItems = 30
            };
            return View(viewModel);
        }
    }
}