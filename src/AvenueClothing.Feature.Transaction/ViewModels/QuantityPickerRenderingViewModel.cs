using System.Web;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class QuantityPickerRenderingViewModel
    {
        public HtmlString QuantityLabel { get; set; }
        public string ProductSku { get; set; }
        public int MaxNumberOfItems { get; set; }
    }
}