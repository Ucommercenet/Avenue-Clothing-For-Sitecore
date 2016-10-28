using System.Collections.Generic;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels.Basket
{
    public class PurchaseOrderViewModel
    {
        public PurchaseOrderViewModel()
        {
            OrderLines = new List<OrderlineViewModel>();
        }
        public IList<OrderlineViewModel> OrderLines { get; set; }

        public string OrderTotal { get; set; }

        public string SubTotal { get; set; }

        public string TaxTotal { get; set; }

        public string DiscountTotal { get; set; }

    }
}