using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace AvenueClothing.Project.Transaction.Services.Impl
{
    public class CurrencyFormattingService: ICurrencyFormatingService
    {
        public string GetFormattedCurrencyString(decimal value, CultureInfo cultureInfo)
        {
            string overriddenCurrencySymbol = GetCurrencySymbol(cultureInfo.ThreeLetterISOLanguageName);
            string cultureSpecificAmount = value.ToString("c", cultureInfo);
            string cultureCurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol;

            return cultureSpecificAmount.Replace(cultureCurrencySymbol, overriddenCurrencySymbol);
        }

        protected virtual string GetCurrencySymbol(string currencyIsoCode)
        {
            // Can only create region infos for specific cultures, not invariant or neutral ones
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.ToString());

                if (regionInfo.ISOCurrencySymbol.Equals(currencyIsoCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return regionInfo.CurrencySymbol;
                }
            }

            return currencyIsoCode;
        }
    }
}