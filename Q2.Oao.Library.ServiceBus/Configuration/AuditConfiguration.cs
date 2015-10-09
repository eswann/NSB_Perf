using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace Q2.Oao.Library.ServiceBus.Configuration
{

	public class AuditConfiguration : IProvideConfiguration<AuditConfig>
	{
		public AuditConfig GetConfiguration()
		{
			//TODO: optionally choose a different audit queue. Perhaps on a remote machine
			//http://docs.particular.net/nservicebus/operations/auditing
			return new AuditConfig
			{
				QueueName = "audit"
			};
		}
	}
}