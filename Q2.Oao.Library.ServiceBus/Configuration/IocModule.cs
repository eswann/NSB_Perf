using Autofac;

namespace Q2.Oao.Library.ServiceBus.Configuration
{
	public class IocModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<BusManager>().As<IBusManager>().SingleInstance();
		}
	}
}