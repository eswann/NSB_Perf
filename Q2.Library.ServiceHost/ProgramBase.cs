using System;
using System.Diagnostics;
using System.ServiceProcess;
using Autofac;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Q2.Oao.Library.Common.Configuration;
using Serilog;

namespace Q2.Library.ServiceHost
{
	public abstract class ProgramBase : ServiceBase
	{
		private readonly ServiceSettings _serviceSettings = new ServiceSettings();
		private readonly IApplicationEnvironment _environment;
		private IConfigurationRoot _configuration;
		private IContainer _iocContainer;

		protected IConfigurationRoot Configuration => _configuration;
		protected IContainer IocContainer => _iocContainer;

		protected Action<string[]> StartAction;
		protected Action StopAction;
		protected Action<ContainerBuilder> ContainerSetup;

		protected ProgramBase(IApplicationEnvironment environment)
		{
			_environment = environment;
		}

		protected void Run(string[] args)
		{
			if (Environment.UserInteractive)
			{
				// running as console app
				var running = false;
				var command = "Start";

				while (true)
				{
					if (command.Equals("Exit", StringComparison.OrdinalIgnoreCase))
					{
						if (running)
						{
							OnStop();
						}
						break;
					}

					if (command.Equals("Start", StringComparison.OrdinalIgnoreCase))
					{
						if (running)
						{
							Console.WriteLine("Already running.");
						}
						else
						{
							OnStart(args);
							running = true;
						}
						Console.WriteLine("Type 'Stop' to stop the service or 'Exit' to exit.");
					}
					else if (command.Equals("Stop", StringComparison.OrdinalIgnoreCase))
					{
						if (running)
						{
							OnStop();
							running = false;
						}
						else
						{
							Console.WriteLine("Already stopped.");
						}
						Console.WriteLine("Type 'Start' to start the service or 'Exit' to exit.");
					}
					command = Console.ReadLine() ?? "";
				}
			}
			else
			{
				Run(this);
			}
		}

		protected override void OnStart(string[] args)
		{
			var eventLog = new EventLog {Source = "Application"};
			eventLog.WriteEntry(_environment.ApplicationName + "- Application Base Path: " + _environment.ApplicationBasePath, EventLogEntryType.Information);

			try
			{
				InitializeConfiguration();
				Debug.WriteLine("Initializing Service: {ServiceName}", _serviceSettings.ServiceName);
				InitializeContainer();
				InitializeLogging();

				Log.Information("Starting Service: {ServiceName}", _serviceSettings.ServiceName);

				if (StartAction == null)
				{
					throw new NotImplementedException("A StartAction must be defined in your 'Program' static constructor.");
				}
				StartAction(args);
				Log.Information("Service: {ServiceName} was started.", _serviceSettings.ServiceName);
			}
			catch (Exception ex)
			{
				if (!Environment.UserInteractive)
				{
					//In case logging is hosed, make sure this goes to the event log
					eventLog.WriteEntry("An error occurred starting the service. " + ex, EventLogEntryType.Error);
				}

				Log.Error(ex, "Service failed to start {ServiceName}", _serviceSettings.ServiceName);
				throw;
			}
		}

		protected override void OnStop()
		{
			try
			{
				Log.Information("Stopping Service: {ServiceName}", _serviceSettings.ServiceName);
				if (StopAction == null)
				{
					Log.Information("Set up the StopAction in your 'Program' static constructor to allow actions on service stop.");
				}
				else
				{
					StopAction();
				}
				Log.Information("Service: {ServiceName} was stopped.", _serviceSettings.ServiceName);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Service failed to stop: {ServiceName}", _serviceSettings.ServiceName);
				throw;
			}
			finally
			{
				_iocContainer?.Dispose();
			}
		}

		private void InitializeConfiguration()
		{
			_configuration = ConfigurationFactory.CreateConfig(_environment);

			IConfigurationSection serviceSettingsConfig = _configuration.GetSection("AppSettings:ServiceSettings");
			serviceSettingsConfig.Bind(_serviceSettings);
		}

		private void InitializeLogging()
		{
			LoggerConfiguration loggerConfig = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Verbose();
			Log.Logger = loggerConfig.CreateLogger();
		}

		private void InitializeContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterInstance(_configuration).As<IConfiguration>().As<IConfigurationRoot>().SingleInstance();
			ContainerSetup?.Invoke(builder);
			_iocContainer = builder.Build();
		}
	}
}