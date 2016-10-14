using System;
using UCommerce.Infrastructure;

namespace AvenueClothing.Project.Website.ExtensionMethods
{
    public static class ObjectFactoryExtensionMethods
    {
        public static object Resolve(this ObjectFactory objectFactory, Type type)
        {
            var obj = typeof(ObjectFactory)
                .GetMethod("Resolve", new Type[] {})
                .MakeGenericMethod(type)
                .Invoke(objectFactory, null);

            return obj;
        }
    }
}