using System.Reflection;
using Microsoft.Dnx.Runtime;
using NServiceBus;
using Q2.Library.ServiceHost;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Library.ServiceBus;
using Q2.Oao.Library.ServiceBus.Pipeline;
using Autofac;
using Q2.Oao.Application.Command.Port.Handlers;
using Q2.Oao.Identity.Messages.Commands;

namespace Q2.Oao.Application.Command.Port
{
	public class Program : ProgramBase
	{
		private static BusManager _busManager;

		public Program(IApplicationEnvironment environment) : base(environment)
		{
			ContainerSetup = builder => builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly(), typeof (IBusManager).Assembly);

			StartAction = args =>
			{
				_busManager = new BusManager(IocContainer, Configuration);
				_busManager.Start(new[] {typeof(StartApplication).Assembly, typeof(CreateUser).Assembly, typeof(StartApplicationHandler).Assembly},
					config =>
					{
						config.RegisterComponents(r => r.ConfigureComponent<OutgoingAppContextMutator>(DependencyLifecycle.InstancePerUnitOfWork));
						config.Pipeline.Register<IncomingAppContextRegistration>();
					});
			};
			StopAction = () => _busManager?.Stop();
		}

		public void Main(string[] args)
		{

			Run(args);
		}

	}
}