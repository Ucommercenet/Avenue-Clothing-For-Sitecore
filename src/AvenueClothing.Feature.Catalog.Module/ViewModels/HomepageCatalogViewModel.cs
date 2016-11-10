using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class HomepageCatalogViewModel
    {
        public HomepageCatalogViewModel()
        {
            Products = new List<ProductCardRenderingViewModel>();
        }

        public IList<ProductCardRenderingViewModel> Products;
    }
}