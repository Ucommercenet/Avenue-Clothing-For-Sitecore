﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Optimization;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.Services.Impl;
using AvenueClothing.Project.Website.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Ucommerce.Api;
using Ucommerce.Catalog;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.GetProduct;
using Ucommerce.Search;
using IUrlService = Ucommerce.Search.Slugs.IUrlService;
using ObjectFactory = Ucommerce.Infrastructure.ObjectFactory;

namespace AvenueClothing.Project.Website
{
    public class AvenueClothingHttpModule : IHttpModule
    {
        private static bool _hasStarted = false;
        private static object _lock = new Object();
        public void Init(HttpApplication context)
        {
            if (!_hasStarted)
            {
                lock (_lock)
                {
                    if (!_hasStarted)
                    {
                        _hasStarted = true;
                        Init();
                    }
                }
            }
        }

        public void Init()
        {
            var services = new ServiceCollection();

            ConfigureUcommerceServices(services);
            ConfigureControllerServices(services);
            ConfigureAcceleratorServices(services);

            var resolver = new ServiceProviderDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void ConfigureAcceleratorServices(ServiceCollection services)
        {
            services.AddTransient<IMiniBasketService, MiniBasketService>();
        }

        public void ConfigureUcommerceServices(IServiceCollection services)
        {
            services.AddTransient(p => ObjectFactory.Instance.Resolve<ITransactionLibrary>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<ICatalogLibrary>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IMarketingLibrary>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<ICatalogContext>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IOrderContext>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IImageService>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<Product>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<Category>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IPipeline<ProductReview>>());
            services.AddTransient(p => Country.All());
	        services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<Country>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IIndex<Ucommerce.Search.Models.Product>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IUrlService>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IProductPriceCalculationService>());
        }

        public void ConfigureControllerServices(IServiceCollection services)
        {
            var controllerTypesToRegister = GetControllerTypesToRegister();
            foreach (var type in controllerTypesToRegister)
            {
                services.AddTransient(type);
            }
        }

        public Type[] GetControllerTypesToRegister(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = BuildManager.GetReferencedAssemblies().OfType<Assembly>().ToArray();
            }

            return (
                    from assembly in assemblies
                    where !assembly.IsDynamic
                    from type in GetExportedTypes(assembly)
                    where typeof(IController).IsAssignableFrom(type)
                    where !type.IsAbstract
                    where !type.IsGenericTypeDefinition
                    where type.Name.EndsWith("Controller", StringComparison.Ordinal)
                    select type)
                .ToArray<Type>();
        }

        public IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException)
            {
                // A type load exception would typically happen on an Anonymously Hosted DynamicMethods
                // Assembly and it would be safe to skip this exception.
                return Type.EmptyTypes;
            }
            catch (FileNotFoundException)
            {
                return Type.EmptyTypes;
            }
            catch (FileLoadException)
            {
                return Type.EmptyTypes;
            }
            catch (TypeLoadException)
            {
                return Type.EmptyTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Return the types that could be loaded. Types can contain null values.
                return ex.Types.Where(type => type != null);
            }
            catch (Exception ex)
            {
                // Throw a more descriptive message containing the name of the assembly.
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    "Unable to load types from assembly {0}. {1}", assembly.FullName, ex.Message), ex);
            }
        }

        public void Dispose()
        {

        }
    }
}