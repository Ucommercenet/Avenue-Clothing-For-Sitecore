using System.Collections.Generic;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels.Basket
{
    public class UpdatedOrderLinesViewModel
    {
        public IList<UpdatedQuantityOrderLineViewModel> UpdatedOrderLines { get; set; }
    }

    public class UpdatedQuantityOrderLineViewModel
    {
        public int Quantity { get; set; }

        public int OrderLineId { get; set; }
    }
}