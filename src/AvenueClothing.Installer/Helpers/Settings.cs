using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Factories;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Security;

namespace AvenueClothing.Installer.Helpers
{
    public class Settings
    {
        private Currency _defaultCurrency;
        private PriceGroup _defaultPriceGroup;
        private IList<Country> _countries = new List<Country>();
        private IList<PaymentMethod> _paymentMethods = new List<PaymentMethod>();

        public void Configure()
        {
            CreateCurrencies();
            CreatePriceGroups();
            CreateOrderNumberSeries();
            CreateCountries();
            CreatePaymentMethods();
            CreateShippingMethods();
            CreateDataTypes();
            CreateProductDefinitions();
            ConfigureEmails();
        }

        private void CreateCurrencies()
        {
            _defaultCurrency = CreateCurrency("EUR", 100);
            CreateCurrency("GBP", 88);
        }

        private Currency CreateCurrency(string isoCode, int exchangeRate)
        {
            var currency = Currency.SingleOrDefault(c => c.ISOCode == isoCode) ?? new Currency();
            currency.Name = isoCode;
            currency.ISOCode = isoCode;
            currency.Deleted = false;
            currency.ExchangeRate = exchangeRate;
            currency.Save();

            return currency;
        }

        private void CreateCountries()
        {
            _countries.Add(CreateCountry("Denmark", "da-DK"));
            _countries.Add(CreateCountry("United Kingdom", "en-GB"));
        }

        private Country CreateCountry(string name, string cultureCode)
        {
            var country = Country.SingleOrDefault(c => c.Name == name) ?? new Country();
            country.Name = name;
            country.Culture = cultureCode;
            country.Deleted = false;
            country.Save();
            return country;
        }

        private void CreatePriceGroups()
        {
            _defaultPriceGroup = CreatePriceGroup("EUR 15 pct", _defaultCurrency, 0.15M);
        }

        private void CreateOrderNumberSeries()
        {
            CreateOrderNumberSeries("Example", "TEST-");
        }

        private void CreateOrderNumberSeries(string name, string prefix)
        {
            var orderNumberSeries = OrderNumberSerie.SingleOrDefault(o => o.OrderNumberName == name) ?? new OrderNumberSerie();
            orderNumberSeries.Name = name;
            orderNumberSeries.OrderNumberName = name;
            orderNumberSeries.Deleted = false;
            orderNumberSeries.Prefix = prefix;
            orderNumberSeries.Increment = 1;
            orderNumberSeries.CurrentNumber = 0;
            orderNumberSeries.Save();
        }

        private PriceGroup CreatePriceGroup(string name, Currency currency, decimal vatRate)
        {
            var priceGroup = PriceGroup.SingleOrDefault(c => c.Name == name) ?? new PriceGroupFactory().NewWithDefaults(name);
            priceGroup.Name = name;
            priceGroup.Currency = currency;
            priceGroup.VATRate = vatRate;
            priceGroup.Deleted = false;
            priceGroup.Save();

            return priceGroup;
        }

        private void ConfigureEmails()
        {
            ConfigureEmailProfiles();

        }

        private void ConfigureEmailProfiles()
        {
            CreateEmailType("OrderConfirmation", "E-mail which will be sent to the customer after checkout.");
            CreateEmailProfile("Default", "OrderConfirmation");
        }

        private void CreateEmailType(string name, string description)
        {
            var emailType = EmailType.SingleOrDefault(x => x.Name == name) ?? new EmailType();
            emailType.Deleted = false;
            emailType.Name = name;
            emailType.Description = description;
            emailType.Save();
        }

        private void CreateEmailProfile(string name, string type)
        {
            var languages = ObjectFactory.Instance.Resolve<ILanguageService>().GetAllLanguages();
            var orderConfirmationType = EmailType.FirstOrDefault(x => x.Name == type);

            var emailProfile = EmailProfile.SingleOrDefault(p => p.Name == name) ?? new EmailProfile();
            emailProfile.Name = name;
            emailProfile.Deleted = false;

            foreach (var language in languages)
            {
                var emailContent = emailProfile.EmailContents.SingleOrDefault(x => x.CultureCode == language.CultureCode && x.EmailType.Name == type) ?? new EmailContent();

                emailContent.ContentId = "0d0c0ed6-7144-4a9a-a7d1-3284c8d459d1";
                emailContent.CultureCode = language.CultureCode;
                emailContent.EmailProfile = emailProfile;
                emailContent.EmailType = orderConfirmationType;
                emailContent.Subject = "OrderConfirmation email";
                emailContent.Save();
            }

            emailProfile.Save();
        }

        private void CreateShippingMethods()
        {
            CreateShippingMethod("Standard (Free)", 0, _defaultCurrency, _defaultPriceGroup);
            CreateShippingMethod("Express", 10, _defaultCurrency, _defaultPriceGroup);
        }

        private void CreateShippingMethod(string name, decimal shippingFee, Currency currency, PriceGroup priceGroup)
        {
            var shippingMethod = ShippingMethod.SingleOrDefault(x => x.Name == name) ?? new ShippingMethodFactory().NewWithDefaults(name);

            var shippingMethodPrice = shippingMethod.ShippingMethodPrices.FirstOrDefault(p => p.PriceGroup.Currency.ISOCode == currency.ISOCode);
            if (shippingMethodPrice == null)
            {
                shippingMethodPrice = new ShippingMethodPrice() { PriceGroup = priceGroup };
                shippingMethod.AddShippingMethodPrice(shippingMethodPrice);
            }
            shippingMethodPrice.Price = shippingFee;
            shippingMethodPrice.PriceGroup = priceGroup;
            shippingMethodPrice.Save();

            shippingMethod.ClearEligibleCountries();
            foreach (var country in _countries)
            {
                shippingMethod.AddEligibleCountry(country);
            }
            shippingMethod.ClearEligibilePaymentMethods();
            foreach (var method in _paymentMethods)
            {
                shippingMethod.AddEligiblePaymentMethod(method);
            }
            shippingMethod.Save();
        }

        private void CreatePaymentMethods()
        {
            _paymentMethods.Add(CreatePaymentMethod("Account", 0, _defaultCurrency, _defaultPriceGroup, 5));
            _paymentMethods.Add(CreatePaymentMethod("Invoice", 0, _defaultCurrency, _defaultPriceGroup, 0));
        }

        private PaymentMethod CreatePaymentMethod(string name, decimal fee, Currency currency, PriceGroup priceGroup, decimal feePercentage)
        {
            var paymentMethod = PaymentMethod.SingleOrDefault(x => x.Name == name) ?? new PaymentMethodFactory().NewWithDefaults(name);
            paymentMethod.Deleted = false;
            paymentMethod.FeePercent = feePercentage;

            var method = paymentMethod.PaymentMethodFees.FirstOrDefault(p => p.PriceGroup.Currency.ISOCode == currency.ISOCode);
            if (method == null)
            {
                method = new PaymentMethodFee() { PriceGroup = priceGroup };
                paymentMethod.AddPaymentMethodFee(method);
            }
            method.Fee = fee;
            method.PriceGroup = priceGroup;
            method.Save();

            paymentMethod.ClearEligibleCountries();
            foreach (var country in _countries)
            {
                paymentMethod.AddEligibleCountry(country);
            }

            var defaultPaymentMethodService = Definition.SingleOrDefault(x => x.Name == "Default Payment Method Service");
            if (defaultPaymentMethodService != null)
            {
                paymentMethod.Definition = defaultPaymentMethodService;
                var acceptUrl = paymentMethod.GetProperty("AcceptUrl");

                if (acceptUrl != null)
                    acceptUrl.SetValue("/confirmation");
            }

            paymentMethod.PaymentMethodServiceName = "Default Payment Method Service";
            paymentMethod.Pipeline = "Checkout";
            paymentMethod.Save();
            return paymentMethod;
        }

        public void AssignAccessPermissionsToDemoStore()
        {
            var userService = ObjectFactory.Instance.Resolve<IUserService>();
            var user = userService.GetCurrentUser();

            var roleService = ObjectFactory.Instance.Resolve<IRoleService>();
            var roles = roleService.GetAllRoles();

            roleService.AddUserToRoles(user, roles);
        }

        private void CreateDataTypes()
        {
            CreateColourDropDownList();
        }

        private void CreateColourDropDownList()
        {
            var dataTypeEnum = CreateDataType("Colour", "Enum");

            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Blue", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Green", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Red", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("White", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("White/Red", dataTypeEnum));
        }

        private DataType CreateDataType(string name, string dataType)
        {
            var dataTypeEnum = DataType.SingleOrDefault(x => x.TypeName == name) ?? new DataTypeFactory().NewWithDefaults(name);

            dataTypeEnum.TypeName = "Colour";
            dataTypeEnum.DefinitionName = dataType;
            dataTypeEnum.Nullable = false;
            dataTypeEnum.ValidationExpression = string.Empty;
            dataTypeEnum.BuiltIn = false;

            dataTypeEnum.Save();

            return dataTypeEnum;
        }

        private DataTypeEnum GenerateColourDataTypeEnum(string colour, DataType parentDataType)
        {
            var dataTypeEnum = DataTypeEnum.SingleOrDefault(x => x.Value == colour && x.DataType.DataTypeId == parentDataType.DataTypeId) ?? new DataTypeEnumFactory().NewWithDefaults(colour);
            dataTypeEnum.Deleted = false;
            dataTypeEnum.DataType = DataType.Get(parentDataType.DataTypeId);
            dataTypeEnum.Save();

            GenericHelpers.DoForEachCulture(language =>
            {
                if (dataTypeEnum.GetDescription(language) == null)
                    dataTypeEnum.AddDescription(new DataTypeEnumDescription { CultureCode = language, DisplayName = colour, Description = colour });
            });

            return dataTypeEnum;
        }

        private void CreateProductDefinitions()
        {
            CreateShirtProductDefinition();
            CreateShoeProductDefinition();
            CreateAccessoryProductDefinition();
        }

        private static ProductDefinition CreateProductDefinition(string name)
        {
            var productDefinition = ProductDefinition.SingleOrDefault(d => d.Name == name) ?? new ProductDefinition();

            productDefinition.Name = name;

            productDefinition.Save();
            return productDefinition;
        }

        private void CreateShirtProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shirt");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "CollarSize", "ShortText", true, true, true, "Collar Inches");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "Colour", "Colour", true, true, true, "Colour");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, false, "Show On Homepage");
        }

        private void CreateShoeProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shoe");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShoeSize", "ShortText", true, true, true, "Shoe Size");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, false, "Show On Homepage");
        }

        private void CreateAccessoryProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Accessory");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, false, "Show On Homepage");
        }

        private void AddProductDefinitionFieldIfDoesntExist(ProductDefinition definition, string name, string typeName, bool displayOnWebsite, bool variantProperty, bool promotoToFacet, string displayName)
        {
            if (definition.GetDefinitionFields().Any(f => f.Name == name))
                return;

            var field = ProductDefinitionField.SingleOrDefault(f => f.Name == name && f.ProductDefinition.ProductDefinitionId == definition.ProductDefinitionId) ?? new ProductDefinitionFieldFactory().NewWithDefaults(name);
            field.Name = name;
            field.DataType = DataType.SingleOrDefault(d => d.TypeName == typeName);
            field.Deleted = false;
            field.Multilingual = false;
            field.DisplayOnSite = displayOnWebsite;
            field.IsVariantProperty = variantProperty;
            field.RenderInEditor = true;
            field.Facet = promotoToFacet;

            //Helpers.DoForEachCulture(language =>
            //    {
            //        if (field.GetDescription(language) == null)
            //            field.AddProductDefinitionFieldDescription(new ProductDefinitionFieldDescription { CultureCode = language, DisplayName = displayName, Description = displayName });
            //    });

            definition.AddProductDefinitionField(field);
            definition.Save();
        }
    }
}
