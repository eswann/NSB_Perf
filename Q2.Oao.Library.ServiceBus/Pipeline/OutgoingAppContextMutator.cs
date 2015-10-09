using NServiceBus;
using NServiceBus.MessageMutator;
using Q2.Oao.Library.Common;
using Q2.Oao.Library.Common.Context;

namespace Q2.Oao.Library.ServiceBus.Pipeline
{
	public class OutgoingAppContextMutator : IMutateOutgoingMessages
	{
		private readonly IAppContext _appContext;
		private readonly IBus _bus;

		public OutgoingAppContextMutator(IBus bus, IAppContext appContext)
		{
			_appContext = appContext;
			_bus = bus;
		}

		public object MutateOutgoing(object message)
		{
			_bus.SetMessageHeader(message, Constants.Headers.CorrelationId, _appContext.CorrelationId.ToString());
			_bus.SetMessageHeader(message, Constants.Headers.TenantId, _appContext.TenantId);
			return message;
		}
	}
}
