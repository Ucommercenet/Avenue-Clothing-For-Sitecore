using System.Collections.Specialized;
using AvenueClothing.Foundation.Installer.Helpers;
using Sitecore.Install.Framework;
using UCommerce.Pipelines;

namespace AvenueClothing.Foundation.Installer.Pipelines.Installation.Tasks
{
    public class CreateUCommerceSettingsTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            var settings = new Settings();
            settings.Configure();

            return PipelineExecutionResult.Success;
        }
    }
}
