using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvenueClothing.Installer.Helpers;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class CreateUCommerceSettings : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var settings = new Settings();
            settings.Configure();
        }
    }
}
