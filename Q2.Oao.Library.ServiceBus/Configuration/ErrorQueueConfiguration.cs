using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace Q2.Oao.Library.ServiceBus.Configuration
{
	public class ErrorQueueConfiguration : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
	{
		public MessageForwardingInCaseOfFaultConfig GetConfiguration()
		{
			return new MessageForwardingInCaseOfFaultConfig
			{
				ErrorQueue = "error"
			};
		}
	}
}
