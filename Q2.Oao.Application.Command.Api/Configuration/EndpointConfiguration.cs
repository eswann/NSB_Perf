using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using Q2.Oao.Application.Messages.Commands;

namespace Q2.Oao.Application.Command.Api.Configuration
{
	public class EndpointConfiguration : IProvideConfiguration<UnicastBusConfig>
	{
		public UnicastBusConfig GetConfiguration()
		{
			//read from existing config 
			var config =  new UnicastBusConfig
				{
					MessageEndpointMappings = new MessageEndpointMappingCollection()
				};
			
			//append mapping to config
			config.MessageEndpointMappings.Add(new MessageEndpointMapping
				{
					AssemblyName = typeof(StartApplication).Assembly.FullName,
					Endpoint = "Q2.Oao.Application.Command.Port"
			});

			return config;
		}
	}
}