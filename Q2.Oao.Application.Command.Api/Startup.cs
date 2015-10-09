using System;
using System.Reflection;
using Autofac;
using IdentityServer3.AccessTokenValidation;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Owin.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NServiceBus;
using Owin;
using Q2.Library.Common.Configuration;
using Q2.Oao.Application.Command.Api.Configuration;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Library.Api;
using Q2.Oao.Library.Api.Context;
using Q2.Oao.Library.Api.Exceptions;
using Q2.Oao.Library.Api.Ioc;
using Q2.Oao.Library.ServiceBus;
using Q2.Oao.Library.ServiceBus.Pipeline;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ILoggerFactory = Microsoft.Framework.Logging.ILoggerFactory;
using LoggerFactory = Microsoft.Owin.Logging.LoggerFactory;

namespace Q2.Oao.Application.Command.Api
{
	public class Startup
	{
		private static readonly LoggingLevelSwitch _loggingLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
		private const string _aureliaAppPolicyName = "AureliaApp";
		private readonly AppSettings _appSettings = new AppSettings();
		private IConfigurationRoot _configuration;

		public Startup(IApplicationEnvironment appEnv)
		{
			InitializeConfig(appEnv);

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Seq(_appSettings.LogSettings.EndpointUri)
				.MinimumLevel.ControlledBy(_loggingLevelSwitch)
				.CreateLogger();
		}

		private void InitializeConfig(IApplicationEnvironment appEnv)
		{
			_configuration = ConfigurationFactory.CreateConfig(appEnv);
			IConfigurationSection appSettingsConfig = _configuration.GetSection("AppSettings");
			appSettingsConfig.Bind(_appSettings);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory, IApplicationLifetime lifetime, IBusManager busManager)
		{
			loggerfactory.AddSerilog();

			Log.Information("Configuring the Application");

			app.UseCors(_aureliaAppPolicyName);

			var options = new IdentityServerBearerTokenAuthenticationOptions
			{
				Authority = _appSettings.IdentityServerAuthority,
				RequiredScopes = new[] { _appSettings.IdentityServerSettings.ClientId },
				NameClaimType = "preferred_username"
			};

			app.UseAppBuilder(
				builder =>
				{
					builder.SetLoggerFactory(LoggerFactory.Default);
					builder.UseIdentityServerBearerTokenAuthentication(options);
				},
				_appSettings.ServiceSettings.ServiceName);

			app.UseMiddleware<ContextMiddleware>();
			app.UseMvc();

			Log.Information("Getting ready to start the bus manager");

			busManager.Start(
				new[] { typeof(StartApplication).Assembly, Assembly.GetExecutingAssembly() },
				config => config.RegisterComponents(r => r.ConfigureComponent<OutgoingAppContextMutator>(DependencyLifecycle.InstancePerUnitOfWork)));

			lifetime.ApplicationStopping.Register(busManager.Stop);

			_loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			services.ConfigureCors(
				options => options.AddPolicy(
					_aureliaAppPolicyName,
					builder =>
					{
						builder.WithOrigins(_appSettings.CorsOrigins);
						builder.AllowAnyMethod();
						builder.AllowAnyHeader();
					}));

			services.AddMvcCore(options => options.Filters.Add(new ExceptionFilter()))
				.AddJsonFormatters(serializerSettings =>
				{
					serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					serializerSettings.NullValueHandling = NullValueHandling.Ignore;
				})

				#warning Related to: https://github.com/aspnet/Mvc/issues/3094, remove after beta 8 released.
				.AddViews();

			services.AddInstance(_configuration);
			services.AddInstance(_appSettings);

			IContainer container = AutofacBootstrapper.CreateContainer(
				services,
				Assembly.GetExecutingAssembly(),
				typeof(IBusManager).Assembly,
				typeof(ContextMiddleware).Assembly);

			return container.Resolve<IServiceProvider>();
		}
	}
}