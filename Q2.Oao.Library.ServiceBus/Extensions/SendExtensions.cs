using System.Threading.Tasks;
using NServiceBus;
using Q2.Oao.Library.Common;
using Q2.Oao.Library.Common.Exceptions;
using Q2.Oao.Library.Common.Messages;

namespace Q2.Oao.Library.ServiceBus.Extensions
{
	public static class SendExtensions
	{

		public static async Task<T> SendCommand<T>(this IBus bus, object command) where T : IResponse
		{
			var response = await bus.Send(command).Register(result => (T)result.Messages[0]).ConfigureAwait(false);
			AssertBusResponse(response);
			return response;
		}

		public static async Task<T> SendLocalCommand<T>(this IBus bus, object command) where T : IResponse
		{
			var response = await bus.SendLocal(command).Register(result => (T)result.Messages[0]).ConfigureAwait(false);
			AssertBusResponse(response);
			return response;
		}

		private static void AssertBusResponse<T>(T response) where T : IResponse
		{
			if (response.Exceptions.SafeAny())
			{
				var commandException = new OaoAggregateException(response.Exceptions);
				commandException.Actions.AddRange(response.Actions);

				throw commandException;
			}
		}
	}
}
