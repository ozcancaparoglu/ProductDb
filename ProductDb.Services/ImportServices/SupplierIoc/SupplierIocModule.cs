using Autofac;
using ProductDb.Services.ImportServices.XmlSupplierServices.SpxServices;

namespace ProductDb.Services.ImportServices.SupplierIoc
{
    class SupplierIocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SpxService>().As<ISupplierService<SpxService>>().InstancePerRequest();
        }
    }
}
