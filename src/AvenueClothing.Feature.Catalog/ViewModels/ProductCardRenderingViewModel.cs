using System.Web;

namespace AvenueClothing.Project.Catalog.ViewModels
{
	public class ProductCardRenderingViewModel
	{
		public string Url { get; set; }
		public string ProductSku { get; set; }
		public string Amount { get; set; }
		public HtmlString DisplayName { get; set; }
		public string CatalogId { get; set; }
		public HtmlString ThumbnailImage { get; set; }
		public ProductPriceRenderingViewModel ProductPriceRenderingViewModel { get; set; }
	}
}