using System.Linq;
using AvenueClothing.Foundation.Installer.Pipelines.Installation;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace AvenueClothing.Foundation.Installer.Pipelines.Initialize
{
    public class RunAvenueClothingInstallerTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<ProductCatalogGroup> _productCatalogGroupRepository;
        private readonly IPipeline<InstallationPipelineArgs> _installationPipeline;
     
        public RunAvenueClothingInstallerTask(IRepository<ProductCatalogGroup> productCatalogGroupRepository, IPipeline<InstallationPipelineArgs> installationPipeline)
        {
            _productCatalogGroupRepository = productCatalogGroupRepository;
            _installationPipeline = installationPipeline;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            if (_productCatalogGroupRepository.Select(x => x.Name == "Avenue-Clothing.com").Any())
            {
                return PipelineExecutionResult.Success;
            }

            return _installationPipeline.Execute(new InstallationPipelineArgs());
        }
    }
}
