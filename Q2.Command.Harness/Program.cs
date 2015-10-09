using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Newtonsoft.Json;
using NServiceBus;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Application.Messages.Responses;
using Q2.Oao.Library.Common.Configuration;
using Q2.Oao.Library.Common.Context;
using Q2.Oao.Library.ServiceBus;
using Q2.Oao.Library.ServiceBus.Extensions;
using Q2.Oao.Library.ServiceBus.Pipeline;

namespace Q2.Command.Harness
{
    public class Program
    {
	    private readonly IApplicationEnvironment _environment;
	    private IConfigurationRoot _configuration;
		private IContainer _iocContainer;
		private readonly ServiceSettings _serviceSettings = new ServiceSettings();
	    private IBus _bus;

		public Program(IApplicationEnvironment environment)
		{
			_environment = environment;
			InitializeConfiguration();
			InitializeContainer();
		}

		public async Task Main(string[] args)
        {
			Console.WriteLine("Hit enter to publish a message to the queue.  Type 'quit' and enter to stop");

			var timer = new Stopwatch();

			while (true)
			{
				var action = Console.ReadLine();

				if (action != null && action.Equals("quit", StringComparison.OrdinalIgnoreCase))
					break;

				var command = new StartApplicationSaga
				{
					FirstName = "First",
					LastName = "Last",
					Email = "first@last.com",
					Password = "password1",
					EligibilityQualifierId = Guid.NewGuid(),
					ProductId = Guid.NewGuid()
				};

				timer.Restart();
				var commandResponse = await _bus.SendCommand<StartApplicationSagaResponse>(command);
				timer.Stop();
				var serialized = JsonConvert.SerializeObject(commandResponse);

				Console.WriteLine(serialized);
				Console.WriteLine("Elapsed (ms): " + timer.ElapsedMilliseconds);
				Console.WriteLine();
				Console.WriteLine();
			}
        }

		private void InitializeConfiguration()
		{
			_configuration = ConfigurationFactory.CreateConfig(_environment);

			IConfigurationSection serviceSettingsConfig = _configuration.GetSection("AppSettings:ServiceSettings");
			serviceSettingsConfig.Bind(_serviceSettings);
		}

		private void InitializeContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterInstance(_configuration).As<IConfiguration>().As<IConfigurationRoot>();
			builder.RegisterInstance(new AppContext {TenantId = "default", CorrelationId = Guid.NewGuid()}).AsImplementedInterfaces();
			builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly(), typeof(IBusManager).Assembly);
			_iocContainer = builder.Build();

			var busManager = _iocContainer.Resolve<IBusManager>();
			_bus = busManager.Start(new[] { typeof(StartApplication).Assembly, Assembly.GetExecutingAssembly() },
					config =>
					{
						config.RegisterComponents(r => r.ConfigureComponent<OutgoingAppContextMutator>(DependencyLifecycle.InstancePerUnitOfWork));
						config.Pipeline.Register<IncomingAppContextRegistration>();
					});
		}
	}
}
