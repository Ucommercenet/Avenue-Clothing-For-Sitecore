using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvenueClothing.Installer.Pipelines.Installation;
using AvenueClothing.Installer.Pipelines.Installation.Tasks;
using Sitecore.Install.Framework;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace AvenueClothing.Installer.Pipelines.Initialize
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
