using System.Web;

namespace AvenueClothing.Feature.Transaction.ViewModels
{
    public class QuantityPickerRenderingViewModel
    {
        public HtmlString QuantityLabel { get; set; }
        public string ProductSku { get; set; }
        public int MaxNumberOfItems { get; set; }
    }
}