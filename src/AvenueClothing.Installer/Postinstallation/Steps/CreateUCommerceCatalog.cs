using System;
using System.Collections.Specialized;
using AvenueClothing.Installer.Helpers;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class CreateUCommerceCatalog : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var catalog = new Catalog("Avenue-Clothing.com", "Demo Catalog");
            catalog.Configure();
        }
    }
}