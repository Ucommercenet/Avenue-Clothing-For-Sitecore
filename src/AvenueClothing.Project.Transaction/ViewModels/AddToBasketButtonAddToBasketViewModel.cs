namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class AddToBasketButtonAddToBasketViewModel
    {
        public int Quantity { get; set; }
        public string ProductSku { get; set; }
        public string VariantSku { get; set; }

		//Commerce Connect
        public string Price { get; set; }
    }
}