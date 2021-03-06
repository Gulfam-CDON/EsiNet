using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Benchmarks
{
    public static class NullPipelineFactory
    {
        public static object Create(Type type)
        {
            return typeof(NullPipelineFactory)
                .GetMethod(nameof(CreateImpl), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(type.GenericTypeArguments.Single())
                .Invoke(null, null);
        }

        private static object CreateImpl<T>()
        {
            return Array.Empty<T>();
        }
    }
}
