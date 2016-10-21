using System;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddToBasket;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Pipelines.UpdateLineItem;
using UCommerce.Runtime;
using UCommerce.Transactions;
using UCommerce.Transactions.Payments;
using UCommerce.Xslt;

namespace AvenueClothing.Tests.Fakes
{
    public class FakeTransactionLibraryInternal : TransactionLibraryInternal
    {
        public FakeTransactionLibraryInternal() : base(null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public Func<bool> HasBasketFunc { get; set; }
        public Func<Basket> GetBasketFunc { get; set; }


        public override bool HasBasket()
        {
            return HasBasketFunc();
        }

        public override Basket GetBasket(bool create = true)
        {
            return GetBasketFunc();
        }
    }
}