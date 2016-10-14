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
using UCommerce.Runtime;
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
            container.Register(() => ObjectFactory.Instance.Resolve<TransactionLibraryInternal>());
            container.Register(() => ObjectFactory.Instance.Resolve<ICatalogContext>());

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