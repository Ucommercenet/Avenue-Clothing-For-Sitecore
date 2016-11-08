using System;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class ProductPriceRenderingViewModel
    {
        public string CalculatePriceUrl { get; set; }
		public string CalculateVariantPriceUrl { get; set; }
        public string SKU { get; set; }
        public Guid CategoryGuid { get; set; }
        public int CatalogGuid { get; set; }
	}
}