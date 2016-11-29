using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class BasketUpdateBasketViewModel
    {
        public BasketUpdateBasketViewModel()
        {
            Orderlines = new List<BasketUpdateOrderline>();
        }

        public IList<BasketUpdateOrderline> Orderlines { get; set; } 

        public string OrderTotal { get; set; }
        public string DiscountTotal { get; set; }
        public string TaxTotal { get; set; }
        public string SubTotal { get; set; }
    }

    public class BasketUpdateOrderline
    {
        public int OrderlineId { get; set; }
        public int Quantity { get; set; }
        public string Total { get; set; }
        public decimal Discount { get; set; }
        public string Tax { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
    }
}