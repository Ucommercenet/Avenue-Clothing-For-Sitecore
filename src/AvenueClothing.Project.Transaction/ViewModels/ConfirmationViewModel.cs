using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class ConfirmationViewModel
    {
        public HtmlString Headline { get; set; }
        public HtmlString Message { get; set; }
        
        public string FirstName { get; set; }

    }
}