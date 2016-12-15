using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvenueClothing.Installer.Postinstallation.Steps;
using Sitecore.Install.Framework;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace AvenueClothing.Installer.Pipelines.Initialize
{
    public class RunAvenueClothingInstallerTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<ProductCatalogGroup> _productCatalogGroupRepository;
        private IList<IPostStep> _composite;
        public RunAvenueClothingInstallerTask(IRepository<ProductCatalogGroup> productCatalogGroupRepository)
        {
            _productCatalogGroupRepository = productCatalogGroupRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            if (_productCatalogGroupRepository.Select(x => x.Name == "Avenue-Clothing.com").Any())
            {
                return PipelineExecutionResult.Success;
            }
            
            _composite = new List<IPostStep>();

            _composite.Add(new CreateUCommerceSettings());
            _composite.Add(new CreateUCommerceCatalog());
            _composite.Add(new UpdateStandardValuesForDefinitions());
            _composite.Add(new RunScratchIndexer());
            _composite.Add(new SynchronizeSitecoreItems());
            _composite.Add(new ClearSitecoreCache());
            _composite.Add(new PublishMasterDatabase());
            _composite.Add(new MoveSitecoreConfigurationFiles());

            foreach (var postStep in _composite)
            {
                postStep.Run(null, null);
            }

            return PipelineExecutionResult.Success;
        }
    }
}
