using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class CreateMarketingCampaign : IPipelineTask<InstallationPipelineArgs>
    {
        private readonly IRepository<Campaign> _campaignRepository;

        public CreateMarketingCampaign(IRepository<Campaign> campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            if (_campaignRepository.Select(x => x.Name == "Demonstration").FirstOrDefault() != null) return PipelineExecutionResult.Success;

            var campaign = CreateCampaign();

            var campaignItem = CreateCampaignItem();
            campaign.AddCampaignItem(campaignItem);

            var relatedProductCampaignItem = CreateRelatedProductCampaignItem();
            
            campaign.AddCampaignItem(relatedProductCampaignItem);
            campaign.Save();

            CreateRelatedProductCampaignTargets(relatedProductCampaignItem);
            CreateRelatedProductAward(relatedProductCampaignItem);

            CreateAward(campaignItem);
            CreateVourcherTarget(campaignItem);

            return PipelineExecutionResult.Success;
        }

        private void CreateRelatedProductAward(CampaignItem relatedProductCampaignItem)
        {
            var relatedProductAward = new PercentOffOrderLinesAward
            {
                CampaignItem = relatedProductCampaignItem,
                Name = "Amount off related product",
                PercentOff = 10m
            };
            relatedProductAward.Save();
        }

        private CampaignItem CreateRelatedProductCampaignItem()
        {
            var relatedProductCampaignItem = new CampaignItem
            {
                AnyTargetAppliesAwards = false,
                AllowNextCampaignItems = true,
                Name = "Related product discount",
                Priority = GetNextPriority(),
                AnyTargetAdvertises = true,
                Enabled = true,
                Definition = Definition.Get(2)
            };

            return relatedProductCampaignItem;
        }

        private void CreateRelatedProductCampaignTargets(CampaignItem relatedProductCampaignItem)
        {
            var relatedproductTarget = new ProductTarget()
            {
                CampaignItem = relatedProductCampaignItem,
                EnabledForApply = true,
                Skus = "19849"
            };
            relatedproductTarget.Save();

            var relatedVoucherTarget = new VoucherTarget()
            {
                CampaignItem = relatedProductCampaignItem,
                Name = "Thank you",
                EnabledForApply = true,
                EnabledForDisplay = false
            };
            var relatedVoucherCode = new VoucherCode()
            {
                VoucherTarget = relatedVoucherTarget,
                MaxUses = 100000,
                NumberUsed = 0,
                Code = "Thank You"
            };
            relatedVoucherTarget.VoucherCodes.Add(relatedVoucherCode);
            relatedVoucherTarget.Save();
        }

        private static void CreateVourcherTarget(CampaignItem campaignItem)
        {
            var voucherTarget = new VoucherTarget();
            voucherTarget.EnabledForDisplay = false;
            voucherTarget.EnabledForApply = true;
            voucherTarget.CampaignItem = campaignItem;
            var voucherCode = new VoucherCode()
            {
                Code = "demo",
                MaxUses = 10000,
                NumberUsed = 0,
                VoucherTarget = voucherTarget
            };
            voucherTarget.VoucherCodes.Add(voucherCode);
            voucherTarget.Name = "target";
            voucherTarget.Save();
        }

        private static void CreateAward(CampaignItem campaignItem)
        {
            var percentOffOrderTotalAward = new PercentOffOrderTotalAward();
            percentOffOrderTotalAward.PercentOff = 10m;
            percentOffOrderTotalAward.CampaignItem = campaignItem;
            percentOffOrderTotalAward.Name = "10% off";
            percentOffOrderTotalAward.Save();
        }

        private CampaignItem CreateCampaignItem()
        {
            var definition = Definition.Get(2);
            var campaignItem = new CampaignItem
            {
                Name = "10% off order total",
                Priority = GetNextPriority(),
                Enabled = true,
                AnyTargetAdvertises = true,
                AnyTargetAppliesAwards = false,
                Definition = definition
            };
            return campaignItem;
        }

        private Campaign CreateCampaign()
        {
            var campaign = new Campaign
            {
                Name = "Demonstration",
                Enabled = true,
                Priority = GetNextPriority(),
                StartsOn = DateTime.Now.AddDays(-1),
                EndsOn = DateTime.Now.AddMonths(1),
            };
            return campaign;
        }

        private int? GetNextPriority()
        {
            var campaign = Campaign.All().OrderByDescending(x => x.Priority).FirstOrDefault();

            if (campaign == null)
                return 1;

            var priority = campaign.Priority;
            if (priority != null)
                priority++;

            return priority;
        }
    }
}
