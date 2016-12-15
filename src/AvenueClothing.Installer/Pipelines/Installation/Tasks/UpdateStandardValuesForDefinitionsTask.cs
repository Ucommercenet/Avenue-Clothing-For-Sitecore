﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore;
using Sitecore.Install.Framework;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Pipelines;
using UCommerce.Sitecore.Entities;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class UpdateStandardValuesForDefinitionsTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
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

            return PipelineExecutionResult.Success;
        }

        private string GetXmlLayoutValueForCategory()
        {
            return
                @"<r xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" >
                    <d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{55AD3180-3D79-461F-AC6F-D679AB58A849}"">
                        <r ds="""" id=""{AA92D865-76BA-4A80-ABD9-BC6E7CE83687}"" par="""" ph=""main"" uid=""{C4458893-3D19-4892-A39B-B1DC965BAD59}"" />
                        <r id=""{878E2B49-315C-4EFE-8D26-52605F78C3D6}"" ph=""header"" uid=""{6CDEF4F3-4422-4B53-887A-2234F0BB10D6}"" />
                        <r id=""{749D4A8D-990A-4FFE-BE3A-C74591EBD862}"" ph=""sidebar"" uid=""{152C6A81-9F8D-42E0-BA64-1641A45E483A}"" />
                        <r id=""{72BE77A8-379E-469F-B1B8-504B7433A300}"" ph=""sidebar"" uid=""{0DA0A7B0-B03B-4B4A-B936-E5AC9756F6A6}"" />
                        <r id=""{BC973506-533C-4611-BC24-C9C0CFCFFB52}"" ph=""main"" uid=""{7C63A0C4-4E84-4BE2-A7AB-F9A145DDDE09}"" />
                        <r id=""{58ECFFD3-2334-4405-B032-EE97F14B2556}"" ph=""main"" uid=""{949573C8-12C8-41C0-9DA8-62064A7F933A}"" />
                        <r ds="""" id=""{EBBE7524-3356-47AC-852A-8EE1DD93CAD6}"" par="""" ph=""header content"" uid=""{E932DB9C-60EC-46AE-95A0-7043A953EBFB}"" />
                        <r id=""{E5C76A34-378F-4808-957B-2C055AE08BCB}"" ph=""header content"" uid=""{F046DE0C-5CC8-4C6D-A921-5F9C5FB9BEA6}"" />
                    </d>
                </r>";

        }

        /// <summary>
        /// Gets the default layout for a product definition (using main layout and product control).
        /// </summary>
        /// <returns>Xml representation of the layout value for template.</returns>
        /// <remarks>As taken directly from the way sitecore renders the xml represe6ntation of the layout for a template.</remarks>
        private string GetXmlLayoutValueForProduct()
        {
            return
                @"<r xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" >
                    <d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{55AD3180-3D79-461F-AC6F-D679AB58A849}"">
                        <r ds="""" id=""{AA92D865-76BA-4A80-ABD9-BC6E7CE83687}"" par="""" ph=""main"" uid=""{D7B4E86E-FC6A-4507-946B-4BB78542EB89}"" />
                        <r id=""{EBBE7524-3356-47AC-852A-8EE1DD93CAD6}"" ph=""header content"" uid=""{4F99DC7B-3906-47C5-9A76-A3EB022999BE}"" />
                        <r ds="""" id=""{878E2B49-315C-4EFE-8D26-52605F78C3D6}"" par="""" ph=""header"" uid=""{8CD2335C-0F4F-4930-89DA-D8C042B5AB94}"" />
                        <r id=""{DF5A237A-6ADB-4526-916B-8ADC251B1A7E}"" ph=""main"" uid=""{A55ABE4A-9E1A-40F9-B279-D8D58C960F2C}"" />
                        <r id=""{2C992D7B-1D72-48DA-8DFC-0E67C4050251}"" ph=""main"" uid=""{E6ACA527-7655-44B0-A809-6C8F49EDFC2E}"" />
                        <r id=""{7D06EF2A-90FF-4B65-BD71-B43DF68CA894}"" ph=""main"" uid=""{2F854156-7D55-4D0E-ABD6-D7502FD1C314}"" />
                        <r ds="""" id=""{89A3DFAE-E473-4520-AAEA-6EEA540352BB}"" par="""" ph=""main"" uid=""{8FEFF25F-C7D0-4C89-8B07-68311E0AEB5E}"" />
                        <r id=""{EA6BD4B8-05CC-487F-AD8B-8E854F473657}"" ph=""main"" uid=""{60E090AF-7140-477B-96E5-FFF4899D1E71}"" />
                        <r id=""{E5C76A34-378F-4808-957B-2C055AE08BCB}"" ph=""header content"" uid=""{2091E847-5ABF-4172-B5D5-1C539F417264}"" />
                        <r id=""{3452011E-8F2A-4AB1-87C3-56FDE6DC5340}"" ph=""main"" uid=""{39DCA080-B374-4F74-97E9-65AD3B4CCC23}"" />
                    </d>
                </r>";

        }
    }
}