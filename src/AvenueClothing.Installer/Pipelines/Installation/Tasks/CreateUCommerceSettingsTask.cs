using AvenueClothing.Installer.Helpers;
using UCommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
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
