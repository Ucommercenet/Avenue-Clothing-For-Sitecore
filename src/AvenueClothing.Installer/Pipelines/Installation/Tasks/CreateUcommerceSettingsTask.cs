using AvenueClothing.Installer.Helpers;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class CreateUcommerceSettingsTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            var settings = new Settings();
            settings.Configure();

            return PipelineExecutionResult.Success;
        }
    }
}
