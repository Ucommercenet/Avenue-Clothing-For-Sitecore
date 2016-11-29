using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class BasketUpdateBasket
    {
        public BasketUpdateBasket()
        {
            RefreshBasket = new List<UpdateOrderLine>();
        }
        public IList<UpdateOrderLine> RefreshBasket { get; set; }
    }

    public class UpdateOrderLine
    {
        public int OrderLineId { get; set; }

        public int OrderLineQty { get; set; }
    }
}