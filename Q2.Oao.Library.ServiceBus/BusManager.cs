using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Autofac;
using Microsoft.Framework.Configuration;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using NServiceBus.Serilog;
using Q2.Oao.Library.Common.Configuration;

namespace Q2.Oao.Library.ServiceBus
{
	public class BusManager : IBusManager
	{
		private readonly ILifetimeScope _lifetimeScope;
		private readonly IConfiguration _configuration;
		private readonly ILog _logger;
		private readonly ServiceSettings _serviceSettings = new ServiceSettings();

		public BusManager(ILifetimeScope lifetimeScope, IConfigurationRoot configuration)
		{
			ServicePointManager.UseNagleAlgorithm = false;
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.DefaultConnectionLimit = 500;

			IConfigurationSection serviceSettingsConfig = configuration.GetSection("AppSettings:ServiceSettings");
			serviceSettingsConfig.Bind(_serviceSettings);

			LogManager.Use<SerilogFactory>();

			_logger = LogManager.GetLogger(_serviceSettings.ServiceName);
			_lifetimeScope = lifetimeScope;
			_configuration = configuration;
		}

		public IBus ServiceBus { get; set; }

		public IBus Start(IEnumerable<Assembly> assembliesToScan, Action<BusConfiguration> configurationAction = null)
		{
			try
			{
				_logger.Info("Starting the Bus!");
				BusConfiguration busConfiguration = Configure(assembliesToScan);
				configurationAction?.Invoke(busConfiguration);
				ServiceBus = Bus.Create(busConfiguration).Start();
			}
			catch (Exception exception)
			{
				_logger.Fatal("Failed to start the bus", exception);
				throw;
			}
			return ServiceBus;
		}

		public void Stop()
		{
			ServiceBus?.Dispose();
		}

		public BusConfiguration Configure(IEnumerable<Assembly> assembliesToScan)
		{
			_logger.Info("Configuring the Bus!");
			string busConnectionString = _configuration["Data:ServiceBusDb:ConnectionString"];
			string transportTypeString = _configuration["Data:ServiceBusType"];
			string transportConnectionString = _configuration[$"Data:{transportTypeString}Transport:ConnectionString"];

			if (busConnectionString == null)
			{
				throw new NullReferenceException("Expected a service bus connection string in configuration location:'Data:ServiceBusDb:ConnectionString' and didn't find one.");
			}

			if (transportConnectionString == null)
			{
				throw new NullReferenceException("Expected a service bus transport connection string in configuration location:'Data:ServiceBusTransport:ConnectionString' and didn't find one.");
			}

			var busConfiguration = new BusConfiguration();
			busConfiguration.EndpointName(_serviceSettings.ServiceName);

			Type transportType;

			switch (transportTypeString.ToLower())
			{
				case "sql":
					transportType = typeof (SqlServerTransport);
					break;
				case "azure":
					transportType = typeof (AzureStorageQueueTransport);
					break;
				case "msmq":
					transportType = typeof (MsmqTransport);
					break;
				default:
					transportType = typeof(RabbitMQTransport);
					break;
			}

			var assemblyList = new List<Assembly>(assembliesToScan)
			{
				typeof(NHibernatePersistence).Assembly,
				transportType.Assembly,
				Assembly.GetExecutingAssembly()
			};
			busConfiguration.AssembliesToScan(assemblyList);
			busConfiguration.UseTransport(transportType).ConnectionString(transportConnectionString);

			//busConfiguration.Transactions().DisableDistributedTransactions();
			busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(busConnectionString);
			
			busConfiguration.UseSerialization<JsonSerializer>();
			busConfiguration.Conventions().DefiningEncryptedPropertiesAs(p => p.Name.Equals("Password", StringComparison.OrdinalIgnoreCase));
			busConfiguration.RijndaelEncryptionService(_serviceSettings.EncryptionKey);

			busConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(_lifetimeScope));
			busConfiguration.EnableInstallers();

			busConfiguration.DefineCriticalErrorAction((message, exception) =>
			{
				string fatalMessage = $"The following critical error was encountered:\n{message}\nProcess is shutting down.";
				_logger.Fatal(fatalMessage, exception);
				if (!Environment.UserInteractive)
				{
					Environment.FailFast(fatalMessage, exception);
				}
			});

			return busConfiguration;
		}
	}
}