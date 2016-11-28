using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;

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
