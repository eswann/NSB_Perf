using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;

namespace Q2.Oao.Library.Common.Configuration
{
	public static class ConfigurationFactory
	{
		public static IConfigurationRoot CreateConfig(IApplicationEnvironment appEnv)
		{
			var configurationBuilder = CreateBuilder(appEnv);
			return configurationBuilder.Build();
		}

		private static IConfigurationBuilder CreateBuilder(IApplicationEnvironment appEnv)
		{
			IConfigurationBuilder configurationBuilder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
				.AddJsonFile("Config.json")
				.AddJsonFile("..\\Config.Bus.json");
			return configurationBuilder;
		}
	}
}