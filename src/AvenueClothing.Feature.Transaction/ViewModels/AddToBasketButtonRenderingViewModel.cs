namespace AvenueClothing.Feature.Transaction.ViewModels
{
    public class AddToBasketButtonRenderingViewModel
    {
        public string AddToBasketUrl { get; set; }
        public string BasketUrl { get; set; }
        public int ConfirmationMessageTimeoutInMillisecs { get; set; }
        public string ConfirmationMessageClientId { get; set; }
        public string ProductSku { get; set; }
        public bool IsProductFamily { get; set; }
    }
}