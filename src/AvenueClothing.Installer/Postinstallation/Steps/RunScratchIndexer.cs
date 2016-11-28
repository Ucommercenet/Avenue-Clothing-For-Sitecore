using System.Collections.Specialized;
using Sitecore.Install.Framework;
using UCommerce.Infrastructure;
using UCommerce.Search.Indexers;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class RunScratchIndexer : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            ObjectFactory.Instance.Resolve<ScratchIndexer>().Index();
        }
    }
}