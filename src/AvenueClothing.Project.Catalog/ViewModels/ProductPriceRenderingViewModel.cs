using System;

namespace AvenueClothing.Project.Catalog.ViewModels
{
    public class ProductPriceRenderingViewModel
    {
        public string CalculatePriceUrl { get; set; }
		public string CalculateVariantPriceUrl { get; set; }
        public string Sku { get; set; }
        public Guid CategoryGuid { get; set; }
        public Guid CatalogGuid { get; set; }
	    public Guid ProductGuid { get; set; }
        public string Price { get; set; }
        public string Tax { get; set; }
    }
}