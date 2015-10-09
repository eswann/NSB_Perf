using System;
using System.Threading.Tasks;
using NServiceBus;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Application.Messages.Responses;
using Q2.Oao.Library.ServiceBus.Handlers;

namespace Q2.Oao.Application.Command.Port.Handlers
{
	public class StartApplicationHandler : CommandHandler<StartApplication, StartApplicationResponse>
	{

		public StartApplicationHandler(IBus bus) : base(bus)
		{
		}

		public async override Task<StartApplicationResponse> HandleCommand(StartApplication command)
		{
			await Task.Delay(100);
			return new StartApplicationResponse
			{
				ApplicationId = Guid.NewGuid()

			};
		}
	}
}