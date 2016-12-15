using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation
{
    public class PostInstallation : IPostStep
    {
        private readonly IList<IPostStep> _composite;

        public PostInstallation()
        {

        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            
        }
    }
}
