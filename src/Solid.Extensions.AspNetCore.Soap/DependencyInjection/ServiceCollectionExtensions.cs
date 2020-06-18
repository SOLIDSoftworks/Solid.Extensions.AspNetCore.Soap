using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Extensions.AspNetCore.Soap;
using Solid.Extensions.AspNetCore.Soap.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Solid_Extensions_AspNetCore_Soap_ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedSoapService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory, Action<SoapServiceBuilder> configure)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Scoped<TService>(factory), configure);

        public static IServiceCollection AddScopedSoapService<TService, TImplementation>(this IServiceCollection services, Action<SoapServiceBuilder> configure)
            where TImplementation : class, TService
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Scoped<TService, TImplementation>(), configure);

        public static IServiceCollection AddSingletonSoapService<TService>(this IServiceCollection services, TService instance, Action<SoapServiceBuilder> configure)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService>(instance), configure);

        public static IServiceCollection AddSingletonSoapService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory, Action<SoapServiceBuilder> configure)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService>(factory), configure);

        public static IServiceCollection AddSingletonSoapService<TService, TImplementation>(this IServiceCollection services, Action<SoapServiceBuilder> configure)
            where TImplementation : class, TService
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService, TImplementation>(), configure);

        public static IServiceCollection ConfigureSoapService<TService>(this IServiceCollection services, Action<SoapServiceOptions> configureOptions)
            => services.Configure(typeof(TService).FullName, configureOptions);

        static IServiceCollection AddSoapService(this IServiceCollection services, ServiceDescriptor descriptor, Action<SoapServiceBuilder> configure)
        {
            var type = descriptor.ServiceType;
            services.Configure<SoapServiceOptions>(type.FullName, options =>
            {
                options.Name = type.GetServiceName();
                options.Namespace = type.GetServiceNamespace();
            });

            var builder = new SoapServiceBuilder(type, services);
            configure(builder);

            services.AddHttpContextAccessor();
            services.AddLogging();

            services.TryAdd(descriptor);
            services.TryAddSingleton<OperationDescriptorFactory>();
            services.TryAddSingleton<MethodLocator>();
            services.TryAddSingleton<MethodInvoker>();
            services.TryAddSingleton<MethodParameterReader>();
            services.TryAddSingleton<SoapContextAccessor>();
            services.TryAddSingleton<ISoapContextAccessor, SoapContextAccessor>();

            return services;
        }
    }
}
