using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Install.Framework;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Sitecore.Entities;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class UpdateStandardValuesForDefinitions : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var sharedFieldRepository = ObjectFactory.Instance.Resolve<IRepository<SharedField>>();
            var sharedFieldsToSave = new List<SharedField>();
            var productDefinitions = ProductDefinition.All().ToList();
            var categoryDefinitions = Definition.All().Where(x => x.DefinitionType == DefinitionType.Get(1)).ToList();

            foreach (var productDefinition in productDefinitions)
            {
                sharedFieldsToSave.Add(new SharedField()
                {
                    FieldValue = GetXmlLayoutValueForProduct(),
                    FieldId = FieldIDs.LayoutField.Guid,
                    ItemId = productDefinition.Guid.Derived("__Standard Values") // This must be set on the Standard Values item. Not the template item directly.
                });
            }

            foreach (var categoryDefinition in categoryDefinitions)
            {
                sharedFieldsToSave.Add(new SharedField()
                {
                    FieldValue = GetXmlLayoutValueForCategory(),
                    FieldId = FieldIDs.LayoutField.Guid,
                    ItemId = categoryDefinition.Guid.Derived("__Standard Values") // This must be set on the Standard Values item. Not the template item directly.
                });
            }

            sharedFieldRepository.Save(sharedFieldsToSave);
        }

        private string GetXmlLayoutValueForCategory()
        {
            return
                @"<r  xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{A43E77F3-FEE9-475A-9DCF-68EFF69B1EEF}""><r id=""{F3745EE1-995F-463E-8638-21384B680DC0}"" ph="""" uid=""{60FA4388-EF95-42C0-BC9D-C0B36ABCF50E}"" /></d></r>";
        }

        /// <summary>
        /// Gets the default layout for a product definition (using main layout and product control).
        /// </summary>
        /// <returns>Xml representation of the layout value for template.</returns>
        /// <remarks>As taken directly from the way sitecore renders the xml represe6ntation of the layout for a template.</remarks>
        private string GetXmlLayoutValueForProduct()
        {
            return
                @"<r  xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{A43E77F3-FEE9-475A-9DCF-68EFF69B1EEF}""><r id=""{3565B2A8-9576-4AA5-8B16-C6399FFAD117}"" ph="""" uid=""{DC0BF419-9937-4FB5-BE39-04BCA8FC741A}"" /></d></r>";
        }
    }
}
