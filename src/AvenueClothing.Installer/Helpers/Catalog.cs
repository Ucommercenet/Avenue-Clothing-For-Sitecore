﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Factories;
using Ucommerce.Infrastructure;
using Ucommerce.Security;

namespace AvenueClothing.Installer.Helpers
{
    public class Catalog
    {
        private string _catalogGroupName;
        private string _catalogName;

        public Catalog(string catalogGroupName, string catalogName)
        {
            _catalogGroupName = catalogGroupName;
            _catalogName = catalogName;
        }

        public void Configure()
        {
            DeleteOldStore();
            var catalogGroup = CreateCatalogGroup();
            var catalog = CreateProductCatalog(catalogGroup);
            EnablePaymentMethodForCatalog(catalogGroup);
            EnableShippingMethodForCatalog(catalogGroup);
            CreateCatalogue(catalog);
            ObjectFactory.Instance.Resolve<IEnsureRolesAreUpToDateService>().EnsureRolesAreUpToDate();
        }

        private void DeleteOldStore()
        {
            var group = ProductCatalogGroup.SingleOrDefault(g => g.Name == "uCommerce.dk");
            if (group != null)
            {
                // Delete products in group
                foreach (var relation in CategoryProductRelation.All().Where(x => group.ProductCatalogs.Contains(x.Category.ProductCatalog)).ToList())
                {
                    var category = relation.Category;
                    var product = relation.Product;
                    category.RemoveProduct(product);
                    product.Delete();
                }

                // Delete catalogs
                foreach (var catalog in group.ProductCatalogs)
                {
                    catalog.Deleted = true;
                }

                // Delete group itself
                group.Deleted = true;
                group.Save();
            }
        }

        private ProductCatalogGroup CreateCatalogGroup()
        {
            var catalogGroupDefinitionType = DefinitionType.SingleOrDefault(x => x.Name == "Product Catalog Groups");
            var catalogGroupDefinitionGuid = Definition.SingleOrDefault(x => x.DefinitionType == catalogGroupDefinitionType).Guid;
            var group = ProductCatalogGroup.SingleOrDefault(c => c.Name == _catalogGroupName) ?? new ProductCatalogGroupFactory().NewWithDefaults(_catalogGroupName, catalogGroupDefinitionGuid);
            group.ProductReviewsRequireApproval = true;
            group.Deleted = false;
            group.CreateCustomersAsMembers = false;
            group.DomainId = null;
            group.Save();
            group.OrderNumberSerie = GetDefaultOrderNumberSeries();
            group.EmailProfile = GetDefaultEmailProfile();
            group.Save();
            return group;
        }

        private EmailProfile GetDefaultEmailProfile()
        {
            var emailProfile = EmailProfile.SingleOrDefault(o => o.Name == "Default");
            if (emailProfile == null)
                throw new ArgumentOutOfRangeException("emailProfile", "The Default email profile could not be found. Have you run the installer?");

            return emailProfile;
        }

        private OrderNumberSerie GetDefaultOrderNumberSeries()
        {
            var orderNumberSeries = OrderNumberSerie.SingleOrDefault(o => o.OrderNumberName == "Example");
            if (orderNumberSeries == null)
                throw new ArgumentOutOfRangeException("orderNumberSeries", "The Example order number series could not be found. Have you run the installer?");

            return orderNumberSeries;
        }

        private ProductCatalog CreateProductCatalog(ProductCatalogGroup catalogGroup)
        {
            var catalogDefinitionType = DefinitionType.SingleOrDefault(x => x.Name == "Product Catalogs");
            var catalogDefinition = Definition.SingleOrDefault(x => x.DefinitionType == catalogDefinitionType);
            var catalog = catalogGroup.ProductCatalogs.SingleOrDefault(c => c.Name == _catalogName)
                          ?? new ProductCatalogFactory().NewWithDefaults(catalogGroup, _catalogName, catalogDefinition.Guid);

            catalog.DisplayOnWebSite = true;
            catalog.Deleted = false;
            catalog.ShowPricesIncludingVAT = true;

            // Versions of CatalogFactory prior to 3.6 did not
            // add catalog to catalog group. Need to do it
            // if not already done to make sure roles and
            // permissions are created properly.
            if (!catalogGroup.ProductCatalogs.Contains(catalog))
                catalogGroup.ProductCatalogs.Add(catalog);

            catalog.Save();

            var priceGroup = PriceGroup.SingleOrDefault(p => p.Name == "EUR 15 pct");
            if (priceGroup != null)
                catalog.PriceGroup = priceGroup;

            catalog.Save();
            return catalog;
        }

        private void EnableShippingMethodForCatalog(ProductCatalogGroup catalogGroup)
        {
            var shippingMethods = ShippingMethod.All();
            foreach (var method in shippingMethods)
            {
                method.ClearEligibleProductCatalogGroups();
                method.AddEligibleProductCatalogGroup(catalogGroup);
                method.Save();
            }
        }

        private void EnablePaymentMethodForCatalog(ProductCatalogGroup catalogGroup)
        {
            var paymentMethods = PaymentMethod.All();
            foreach (var method in paymentMethods)
            {
                method.ClearEligibleProductCatalogGroups();
                method.AddEligibleProductCatalogGroup(catalogGroup);
                method.Save();
            }
        }

        private void CreateCatalogue(ProductCatalog catalog)
        {
            CreateShirts(catalog);
            CreateShoes(catalog);
            CreateAccessories(catalog);
        }

        private void CreateShirts(ProductCatalog catalog)
        {
            var tops = CreateCatalogCategory(catalog, "Tops", "A9413551-70E7-491B-9A10-09E16C961329");
            var shirtDefinition = GetProductDefinition("Shirt");
            CreateFormalShirts(tops, shirtDefinition);
            CreateCasualShirts(tops, shirtDefinition);
        }

        private void CreateCasualShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var casual = CreateChildCategory(shirts, "Casual", "0468BEFA-2823-45DA-8B80-0CA40B0A652D");

            var prettyGreen = CreateProductOnCategory(casual, shirtDefinition, "GHXG-4044-7604-1", "Pretty Green White Pinstripe Jersey Short Sleeve Polo Shirt", 65, "C25DADDD-AFC8-4722-A615-1D19CE70DE38", "", "<ul><li>Three button placket and classic polo collar</li><li>Short sleeved</li><li>Signature embroidered Pretty Green chest badge</li><li>White and black pin stripe</li><li>100% jersey cotton</li><li>Style number : GHXG/4044/7604/1</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/polo-shirts-7/pretty-green-white-pinstripe-jersey-short-19759.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(prettyGreen, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L" }, new List<string>() { "White/Red" });
        }

        private void CreateFormalShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var formal = CreateChildCategory(shirts, "Formal", "2EC799B0-622D-49B9-AF01-13D84A3F5589");

            var smDesc = "<p>100% high quality cotton fabric from Austria &amp; Italy</p> <p>European made</p> <p>Unique and exclusive patterns - on the inside of the collar, placket and cuffs</p> <p>Non-iron fabric</p> <p>Machine washable to 40<span>°</span>&nbsp;C</p> <p>Slim cut tapered shirt</p> <p>Cuffs close with or without cufflinks</p> <p>Permanent collar stays</p> <p>Versatility - smart for the office and exclusive for after work</p>";

            var wonderland = CreateProductOnCategory(formal, shirtDefinition, "BWWMSFSS-LE", "Black & White Wonderland Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219, "0D8E4E64-911E-4FC2-9304-ABA8A4139EEE", "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/black--white-wonderland-mood-slim-fit-signature-shirt---limited-edition-%5b200-pieces%5d/c-23/c-85/p-235\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(wonderland, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White" });
            AddProductProperty(wonderland, "ShowOnHomepage", "true");
            wonderland.Guid = new Guid("E43EFAD8-65B7-4F9A-87DD-B1EEEE469E21");
            wonderland.Save();

            var kittens = CreateProductOnCategory(formal, shirtDefinition, "BKMSFSS", "Blue Kittens Mood Slim Fit Signature Shirt", 179, "DC919B32-8C9F-4837-9227-05FF41DC7AA1", "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/blue-kittens-mood-slim-fit-signature-shirt/c-23/c-85/p-119\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(kittens, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var pink = CreateProductOnCategory(formal, shirtDefinition, "PMMSFSS-LE", "Pink Manga Mood Slim Fit Signature Shirt - Limited Edition [100 pieces]", 279, "E9AFD293-3C29-4D0B-A4B6-B539B264AA19", "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/pink-manga-mood-slim-fit-signature-shirt---limited-edition-%5b100-pieces%5d/c-23/c-85/p-156\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(pink, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var jungle = CreateProductOnCategory(formal, shirtDefinition, "JNMSFSS", "Jungle by Night Mood Slim Fit Signature Shirt", 179, "4D7ADA13-5CB0-404A-94C0-BD4E9605C1E3", "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/jungle-by-night-mood-slim-fit-signature-shirt/c-23/c-85/p-125\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(jungle, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var ghoulies = CreateProductOnCategory(formal, shirtDefinition, "GGMSFSS-LE", "Green Ghoulies Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219, "9DFDD6BA-33B7-494A-BF05-A15CCE811EE0", "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/green-ghoulies-mood-slim-fit-signature-shirt---limited-edition-%5b200-pieces%5d/c-23/c-85/p-124\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(ghoulies, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var comic = CreateProductOnCategory(formal, shirtDefinition, "CMSFSS", "Comic Mood Slim Fit Signature Shirt", 179, "1BF03C01-9F41-4BC1-9BAE-B66D76E48F6D", "", smDesc + "<p>As featured on <a href=\"https:///shop/all-moods/comic-mood-slim-fit-signature-shirt/c-23/c-85/p-151\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(comic, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var eton = CreateProductOnCategory(formal, shirtDefinition, "2285794682539", "Eton Purple & White Stripe Contemporary Fit Formal Dress Shirt", 135, "EE0A24FC-BF44-45A0-9796-8A36815D7F33", "", "<ul><li>Contemporary Fit</li><li>Single button cuffs with double button holes for cuff links</li><li>Pointed collar with bone inserts</li><li>Purple and white stipe</li><li>100% cotton</li><li>Style number : 2285794682539</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/eton-purple-white-stripe-contemporary-formal-20625.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(eton, "Striped", new List<string>() { "15", "15.5", "16", "16.5" }, new List<string>() { "White" });
            AddProductProperty(eton, "ShowOnHomepage", "true");
            eton.Guid = new Guid("6DCCB368-4B3A-4075-B989-D4498A4F6C42");
            eton.Save();

            var rlA02 = CreateProductOnCategory(formal, shirtDefinition, "A02 WCJNK C0223 C421A", "Polo Ralph Lauren Blue & White Stripe Custom Fit Regent Poplin Dress Shirt", 85, "E104825F-D067-4CCE-9A45-DEA8C6595DA6", "", "<ul><li>Custom fit dress shirt</li><li>Classic single button collar</li><li>Long sleeve with single button cuff</li><li>Blue and white stripe with newport navy polo player</li><li>100% poplin cotton</li><li>Style number : A02 WCJNK C0223 C421A</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/polo-ralph-lauren-blue-white-stripe-18999.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(rlA02, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L", "XXL", "XXXL" }, new List<string>() { "Blue" });

            var rlA04 = CreateProductOnCategory(formal, shirtDefinition, "A04 WCBPS C4572 G4562", "Polo Ralph Lauren Blue & Navy Stripe Custom Fit Broadcloth Long Sleeve Shirt", 105, "B0236599-415F-4771-A934-15F74D2B29D3", "", "<ul><li>Custom Fit</li><li>Classic button down collar</li><li>Long sleeve with single button cuff</li><li>Blue, navy and white stripes</li><li>100% cotton</li><li>Style number : A04 WCBPS C4572 G4562</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/polo-ralph-lauren-blue-navy-stripe-18980.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(rlA04, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L", "XXL" }, new List<string>() { "Blue" });
        }

        private void CreateShoes(ProductCatalog catalog)
        {
            var shoes = CreateCatalogCategory(catalog, "Shoes", "F161B9C5-BC86-43CD-8350-880DB938A288");

            var shoeDefinition = GetProductDefinition("Shoe");

            var hiking = CreateProductOnCategory(shoes, shoeDefinition, "074617", "Paraboot Avoriaz/Jannu Marron Brut Marron Brown Hiking Boot Shoes", 343.85M, "DE2C86A7-C38F-4445-9598-07EB104E45BE", "", "<ul><li>Paraboot Avoriaz Mountaineering Boots</li><li>Marron Brut Marron (Brown)</li><li>Full leather inners and uppers</li><li>Norwegien Welted Commando Sole</li><li>Hand made in France</li><li>Style number : 074617</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-avoriaz-jannu-marron-brut-brown-20879.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(hiking, new List<string>() { "6", "8", "10" });
            AddProductProperty(hiking, "ShowOnHomepage", "true");
            hiking.Guid = new Guid("1B27B4B8-C9C2-4CFE-B37A-2CCD86F9CBBB");
            hiking.Save();

            var marron = CreateProductOnCategory(shoes, shoeDefinition, "710708", "Paraboot Chambord Tex Marron Lis Marron Brown Shoes", 281.75M, "A9B7C1EF-6156-4772-AB24-DD8BD4E49328", "", "<ul><li>Style : Chambord Tex</li><li>Colour : Marron Lis Marron</li><li>Paraboot style code : 710708</li><li>Estimated delivery time : 1 - 4 weeks</li><li>Customers ordering from outside the EU will receive a 20% VAT discount on their order. This is applied at checkout once you have given your delivery details.</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-order-chambord-marron-brown-shoes-20709.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(marron, new List<string>() { "6", "8", "10" });

            var brown = CreateProductOnCategory(shoes, shoeDefinition, "710707", "Paraboot Chambord Tex Marron Lis Cafe Brown Shoes", 281.75M, "CF3E318F-1AA9-4B3B-A902-8616529CC2B2", "", "<ul><li>Style : Chambord Tex</li><li>Colour : Marron Lis Cafe</li><li>Paraboot style code : 710707</li><li>Estimated delivery time : 1 - 4 weeks</li><li>Customers ordering from outside the EU will receive a 20% VAT discount on their order. This is applied at checkout once you have given your delivery details.</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-order-chambord-marron-cafe-brown-18606.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(brown, new List<string>() { "6", "8", "10", "12" });
        }

        private void CreateAccessories(ProductCatalog catalog)
        {
            var accessories = CreateCatalogCategory(catalog, "Accessories", "54784087-30C8-4670-9B45-AFCD46F29511");

            var accessoryDefinition = GetProductDefinition("Accessory");

            CreateProductOnCategory(accessories, accessoryDefinition, "LBAT", "Luxury Baby Alpaca Throw", 125, "5DF6155C-C457-4ADC-8868-BD1E0A728B9F", "", "<ul><li>100% Baby Alpaca Wool</li><li>180cm x 130cm (plus tassel fringe)</li><li>Fair Trade</li><li>Hypo-allergenic</li></ul><p>As featured on <a href=\"http://www.amazon.co.uk/Luxury-Threads-100%25-Alpaca-Throw/dp/B008WMLGCO/ref=sr_1_5?s=kitchen&ie=UTF8&qid=1349966612&sr=1-5\">Luxury Threads</a></p>");

            CreateTies(accessories, accessoryDefinition);
            CreateScarves(accessories, accessoryDefinition);
        }

        private void CreateScarves(Category accessories, ProductDefinition accessoryDefinition)
        {
            var scarves = CreateChildCategory(accessories, "Scarves", "4F79D9C0-7068-4B70-BE47-B79C880AD399");
            CreateProductOnCategory(scarves, accessoryDefinition, "LBAS", "Luxury Baby Alpaca Scarf", 99, "75F07755-4499-4D35-A95C-FDAA1C448E4E", "<ul><li>100% Baby Alpaca Wool</li><li>180cm x 130cm (plus tassel fringe)</li><li>Fair Trade</li><li>Hypo-allergenic</li></ul><p>As featured on <a href=\"http://www.amazon.co.uk/Luxury-Threads-100%25-Alpaca-Throw/dp/B008WMLGCO/ref=sr_1_5?s=kitchen&ie=UTF8&qid=1349966612&sr=1-5\">Luxury Threads</a></p>");
            CreateProductOnCategory(scarves, accessoryDefinition, "20178", "Hugo Boss Black Frando Multi Colour Square Check Pattern Wool Scarf", 55, "D8951690-0B7B-4EFA-991F-7F710663068A", "<ul><li>Embroidered Hugo Boss Logo</li><li>Tassled Ends</li><li>Size 180 cm x 29 cm (including tassels)</li><li>Multi Colour Square Check Pattern</li><li>100% Virgin Wool</li><li>Style Number : 50230552 10159101 01 641</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/scarves-32/hugo-boss-black-frando-multi-colour-20178.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(scarves, accessoryDefinition, "20180", "Hugo Boss Black Farion Multi Colour Stripe ", 55, "FE3D4DB8-80A9-4CB7-B14D-10BE7F6C6C86", "<ul><li>Embroidered Hugo Boss logo</li><li>Tasselled ends</li><li>Size 180 cm x 25 cm (including tassels)</li><li>Multi Colour Stripe</li><li>100% Knitted Wool</li><li>Style Number : 50226493 10157685 01 960</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/scarves-32/hugo-boss-black-farion-multi-colour-20180.htm\">Pritchards.co.uk</a></p>");
        }

        private void CreateTies(Category accessories, ProductDefinition accessoryDefinition)
        {
            var ties = CreateChildCategory(accessories, "Ties", "");
            CreateProductOnCategory(ties, accessoryDefinition, "19849", "Paul Smith Accessories Classic Blue with Brown & Pink Stripe Silk Woven Tie", 69.50M, "D5F7C380-9E1A-425C-B49E-2E75D6A8C2C7", "", "<ul><li>Classic Stripe Tie</li><li>Tie length : 140cm</li><li>Blade width : 9cm </li><li>Blue with brown and pink stripes</li><li>100% silk (Made in Italy)</li><li>Style Number : AGXA/764L/R38/VP</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/paul-smith-accessories-classic-blue-with-19849.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(ties, accessoryDefinition, "19324", "Hugo Boss Black Dark Red Ultra Slim Knitted Tie", 65, "AE82EC87-391A-4B78-907F-3992BB79A55D", "", "<ul><li>Ultra slim knitted tie</li><li>Length: 145 cm </li><li> Width: 5 cm</li><li>Squared end</li><li>100% High-quality wool</li><li>Style Number : 5023476 10156986 01 504</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/hugo-boss-black-burgundy-ultra-slim-19324.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(ties, accessoryDefinition, "20301", "Hugo Boss Black Pink & White Flower Pattern Silk Woven Tie", 65, "2A6BC0FF-24E6-4D2E-B87D-08899295ADAE", "Silky soft tie from Hugo Boss", "<ul><li>Flower Pattern Silk Tie</li><li>Tie Length : 145cm</li><li>Blade Width : 7.5cm</li><li>Black With Pink &amp; White Flowers</li><li>100% Silk (Made in Italy)</li><li>Style Number : 50227733 10156957 01 001</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/hugo-boss-black-pink-white-flower-20301.htm\">Pritchards.co.uk</a></p>");
        }

        private static ProductDefinition GetProductDefinition(string name)
        {
            var definition = ProductDefinition.SingleOrDefault(d => d.Name == name);

            if (definition == null)
                throw new ArgumentOutOfRangeException("definition", string.Format("No product definition with the name \"{0}\" could be found. Please check you have installed the default settings.", name));

            return definition;
        }

        private void AddShirtVariantsToProduct(Product product, string namePrefix, IList<string> sizes, IList<string> colours)
        {
            if (!sizes.Any())
                return;

            foreach (var size in sizes)
            {
                foreach (var colour in colours)
                {
                    AddShirtVariantToProduct(product, namePrefix, size, colour);
                }
            }
        }

        private void AddShoeVariantsToProduct(Product product, IList<string> sizes)
        {
            if (!sizes.Any())
                return;

            foreach (var size in sizes)
            {
                AddShoeVariantToProduct(product, size);
            }
        }

        private void AddShoeVariantToProduct(Product product, string size)
        {
            var sku = string.Format("{0}-{1}", product.Sku, size);
            var variant = CreateVariantOnProduct(product, sku, size, "");
            AddProductProperty(variant, "ShoeSize", string.Format("UK {0}", size));
            variant.Save();
        }

        private void AddShirtVariantToProduct(Product product, string namePrefix, string size, string colour)
        {
            var sku = string.Format("{0}-{1}-{2}", product.Sku, size, colour);
            var name = string.Format("{0} {1}\" {2}", namePrefix, size, colour);

            var variant = CreateVariantOnProduct(product, sku, name, "");
            AddProductProperty(variant, "CollarSize", size);
            AddProductProperty(variant, "Colour", colour);
            variant.Save();
        }

        private void AddProductProperty(Product product, string dataFieldName, string value)
        {
            if (product.ProductProperties.Any(p => p.ProductDefinitionField.Name == dataFieldName))
                return;

            var field = GetProductDefinitionField(product, dataFieldName);
            product.AddProductProperty(new ProductProperty
            {
                ProductDefinitionField = field,
                Value = value
            });
        }

        private ProductDefinitionField GetProductDefinitionField(Product product, string name)
        {
            var field = ProductDefinitionField.SingleOrDefault(d => product.ProductDefinition.Name == d.ProductDefinition.Name && d.Name == name);

            if (field == null)
                throw new ArgumentOutOfRangeException("field", string.Format("No product definition field with the name \"{0}\" could be found. Please check you have installed the default settings.", name));

            return field;
        }

        private Category CreateCatalogCategory(ProductCatalog catalog, string name, string imageId)
        {
            var category = CreateCategory(catalog, name, imageId);
            catalog.AddCategory(category);
            return category;
        }

        private Category CreateChildCategory(Category parent, string name, string imageId)
        {
            var category = CreateCategory(parent.ProductCatalog, name, imageId);
            parent.AddCategory(category);
            return category;
        }

        private static Category CreateCategory(ProductCatalog catalog, string name, string imageId)
        {
            var definition = GetDefaultDefinition();
            var category = Category.SingleOrDefault(c => c.Name == name) ?? new CategoryFactory().NewWithDefaults(catalog, definition, name);
            category.DisplayOnSite = true;

            GenericHelpers.DoForEachCulture(language =>
            {
                if (category.GetDescription(language) == null)
                    category.AddCategoryDescription(new CategoryDescription() { CultureCode = language, DisplayName = name });
            });

            category.ImageMediaId = imageId;
            category.Save();
            return category;
        }

        private static Definition GetDefaultDefinition()
        {
            var definition = Definition.SingleOrDefault(d => d.Name == "Default Category Definition");
            if (definition == null)
                definition = new Definition { Name = "Default Category Definition", DefinitionType = DefinitionType.Get(1), Deleted = false, Guid = Guid.NewGuid(), SortOrder = 1, Description = "Default Category Definition" };
            definition.Save();
            return definition;
        }

        private Product CreateProductOnCategory(Category category, ProductDefinition productDefinition, string sku, string name, decimal price, string imageId, string shortDescription = "", string longDescription = "")
        {
            var product = CreateBaseProduct(productDefinition, sku, null, name, imageId);

            GenericHelpers.DoForEachCulture(
                language =>
                {
                    if (!product.HasDescription(language))
                        product.AddProductDescription(new ProductDescription()
                        {
                            CultureCode = language,
                            DisplayName = name,
                            ShortDescription = shortDescription,
                            LongDescription = longDescription
                        });
                });


            CreateProductPricesForProduct(category, price, product);

            // uCommerce checks whether the product already exists in the create
            // when creating the new relation.
            product.AddCategory(category, 0);

            product.Save();

            return product;
        }

        private void CreateProductPricesForProduct(Category category, decimal amount, Product product)
        {
            var price = new Price() { Amount = amount, Guid = Guid.NewGuid(), PriceGroup = category.ProductCatalog.PriceGroup };
            product.ProductPrices.Add(new ProductPrice()
            {
                Guid = Guid.NewGuid(),
                MinimumQuantity = 1,
                Price = price,
                Product = product
            });
        }

        private Product CreateVariantOnProduct(Product product, string variantSku, string name, string imageId)
        {
            var variant = CreateBaseProduct(product.ProductDefinition, product.Sku, variantSku, name, imageId);
            product.AddVariant(variant);
            return variant;
        }

        private Product CreateBaseProduct(ProductDefinition productDefinition, string sku, string variantSku, string name, string imageId)
        {
            if (sku.Length > 30)
                sku = sku.Substring(0, 30);

            if (!String.IsNullOrWhiteSpace(variantSku) && variantSku.Length > 30)
                variantSku = variantSku.Substring(0, 30);

            var product = Product.SingleOrDefault(p => p.Sku == sku && p.VariantSku == variantSku) ?? new Product();

            product.Sku = sku;
            product.VariantSku = variantSku;
            product.Name = name;
            product.ProductDefinition = productDefinition;
            product.DisplayOnSite = true;
            product.AllowOrdering = true;
            product.PrimaryImageMediaId = imageId;
            product.ThumbnailImageMediaId = imageId;
            product.Save();

            return product;
        }
    }
}
