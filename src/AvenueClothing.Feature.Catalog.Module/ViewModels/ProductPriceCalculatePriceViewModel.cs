using System;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class ProductPriceCalculatePriceViewModel
    {
        public string ProductSku { get; set; }
        public int CatalogId { get; set; }
        public Guid CategoryId { get; set; }
    }
}