﻿using Ucommerce;

namespace AvenueClothing.Project.Transaction.ViewModels
{
	public class MiniBasketRenderingViewModel
	{
		public int NumberOfItems { get; set; }
		public Money Total { get; set; }
		public bool IsEmpty { get; set; }
		public string RefreshUrl { get; set; }
	}
}