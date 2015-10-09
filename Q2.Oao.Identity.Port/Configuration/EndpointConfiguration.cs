using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using Q2.Oao.Identity.Messages.Commands;

namespace Q2.Oao.Identity.Port.Configuration
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
					AssemblyName = typeof(CreateUser).Assembly.FullName,
					Endpoint = "Q2.Oao.Identity.Port"
				});
			return config;
		}
	}
}