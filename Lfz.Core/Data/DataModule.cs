using System.Reflection;
using Autofac;
using PMSoft.Data.Providers;
using Module = Autofac.Module;

namespace PMSoft.Data {
    /// <summary>
    /// 数据模块，提供Autofac依赖注入实现
    /// </summary>
    public class DataModule : Module {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder) { 
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency(); 
        }

        /// <summary>
        /// Override to attach module-specific functionality to a
        ///             component registration.
        /// </summary>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component
        ///             registrations - ordering is not important.
        /// </remarks>
        /// <param name="componentRegistry">The component registry.</param><param name="registration">The registration to attach functionality to.</param>
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration) {
            if (typeof(IDataServicesProvider).IsAssignableFrom(registration.Activator.LimitType)) {
                var propertyInfo = registration.Activator.LimitType.GetProperty("ProviderName", BindingFlags.Static | BindingFlags.Public);
                if (propertyInfo != null) {
                    registration.Metadata["ProviderName"] = propertyInfo.GetValue(null, null);
                }
            }
        }
    }
}