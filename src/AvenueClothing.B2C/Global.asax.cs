using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using UCommerce.Infrastructure;
using AvenueClothing.Project.Website.ExtensionMethods;
using UCommerce.Catalog;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;
using UCommerce.Search;
using UCommerce.Transactions;

namespace AvenueClothing.Project.Website
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // Register your types, for instance:
            //container.Register<IUserRepository, SqlUserRepository>(Lifestyle.Scoped);

            //Register uCommerce types
            container.Register(() => ObjectFactory.Instance.Resolve<TransactionLibraryInternal>(), Lifestyle.Transient);
            container.Register(() => ObjectFactory.Instance.Resolve<CatalogLibraryInternal>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<SearchLibraryInternal>(), Lifestyle.Transient);
            container.Register(() => ObjectFactory.Instance.Resolve<ICatalogContext>(), Lifestyle.Transient);
            container.Register(() => ObjectFactory.Instance.Resolve<IOrderContext>(), Lifestyle.Transient);
            container.Register(() => ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<IImageService>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<IRepository<Product>>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<IRepository<Category>>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>(), Lifestyle.Transient);
			container.Register(() => ObjectFactory.Instance.Resolve<IPipeline<ProductReview>>(), Lifestyle.Transient);
            container.Register(() => Country.All(), Lifestyle.Transient);
            
            //var uCommerceExportedTypes = AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(assembly => assembly.FullName.StartsWith("UCommerce"))
            //    .SelectMany(x => x.GetExportedTypes())
            //    .Where(type => !type.IsValueType)
            //    .Where(type => !type.IsGenericTypeDefinition);
            //foreach (var type in uCommerceExportedTypes)
            //{
            //    container.Register(type, ()=> ObjectFactory.Instance.Resolve(type));
            //}

            // This is an extension method from the integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}