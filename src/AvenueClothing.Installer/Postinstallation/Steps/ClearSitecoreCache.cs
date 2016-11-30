using System.Collections.Specialized;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class ClearSitecoreCache : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            CacheManager.ClearAllCaches();
            Factory.Reset();
        }
    }
}