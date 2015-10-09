using Autofac;
using Microsoft.Framework.Configuration;

namespace Q2.Oao.Identity.Port.Configuration
{
	public class IocModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(ctx =>
			{
				var config = ctx.Resolve<IConfigurationRoot>();
				IConfigurationSection appSettingsConfig = config.GetSection("AppSettings");
				var appSettings = new AppSettings();
				appSettingsConfig.Bind(appSettings);
				return appSettings;
			}).AsSelf().SingleInstance();

		}
	}
}