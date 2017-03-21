using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvenueClothing.Project.Transaction.Services
{
    public interface ICurrencyFormatingService
    {
        string GetFormattedCurrencyString(decimal value, CultureInfo cultureInfo);
    }
}
