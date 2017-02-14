using System;

namespace AvenueClothing.Project.Catalog.ViewModels
{
    public class ProductPriceCalculatePriceViewModel
    {
        public string ProductSku { get; set; }
        public int CatalogId { get; set; }
        public Guid CategoryId { get; set; }
    }
}