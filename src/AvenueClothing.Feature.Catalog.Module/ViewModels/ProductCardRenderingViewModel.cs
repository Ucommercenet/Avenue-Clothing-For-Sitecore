using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class ProductCardRenderingViewModel
	{
		public string Url { get; set; }

		public string ProductSku { get; set; }
		public string	Amount { get; set; }

        public HtmlString DisplayName { get; set; }
    }
}