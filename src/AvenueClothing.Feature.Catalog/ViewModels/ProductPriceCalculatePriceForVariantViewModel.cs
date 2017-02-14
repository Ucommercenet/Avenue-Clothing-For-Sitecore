using System;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class ProductPriceCalculatePriceForVariantViewModel
    {
        public string ProductSku { get; set; }
        public string ProductVariantSku { get; set; }
        public int CatalogId { get; set; }
        public Guid CategoryId { get; set; }
    }
}