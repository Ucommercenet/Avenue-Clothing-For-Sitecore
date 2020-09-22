﻿using System.Linq;
using AvenueClothing.Installer.Pipelines.Installation;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

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
