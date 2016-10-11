namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
    public class AddToBasketButtonViewModel
    {
        public string AddToBasketUrl { get; set; }
        public string ValidateProductExistsUrl { get; set; }
        public string BasketUrl { get; set; }
        public int ConfirmationMessageTimeoutInMillisecs { get; set; }
        public string ConfirmationMessageClientId { get; set; }
        
    }
}