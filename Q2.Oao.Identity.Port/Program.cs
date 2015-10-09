using System.Reflection;
using Autofac;
using Microsoft.Dnx.Runtime;
using NServiceBus;
using Q2.Library.ServiceHost;
using Q2.Oao.Identity.Messages.Commands;
using Q2.Oao.Identity.Port.Handlers;
using Q2.Oao.Library.ServiceBus;
using Q2.Oao.Library.ServiceBus.Pipeline;

namespace Q2.Oao.Identity.Port
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
				_busManager.Start(new[] {typeof(CreateUser).Assembly, typeof(CreateUserHandler).Assembly},
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