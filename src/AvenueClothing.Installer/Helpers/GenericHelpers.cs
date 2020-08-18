using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Globalization;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Language = Ucommerce.Infrastructure.Globalization.Language;

namespace AvenueClothing.Installer.Helpers
{
    public class GenericHelpers
    {
        public static void DoForEachCulture(Action<string> toDo)
        {
            foreach (Language language in ObjectFactory.Instance.Resolve<ILanguageService>().GetAllLanguages())
            {
                toDo(language.CultureCode);
            }
        }
    }
}
