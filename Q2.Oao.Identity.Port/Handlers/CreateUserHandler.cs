using System;
using System.Threading.Tasks;
using Q2.Oao.Identity.Messages.Commands;
using Q2.Oao.Identity.Messages.Responses;
using NServiceBus;
using Q2.Oao.Library.ServiceBus.Handlers;

namespace Q2.Oao.Identity.Port.Handlers
{
	public class CreateUserHandler : CommandHandler<CreateUser, CreateUserResponse>
    {

	    public CreateUserHandler(IBus bus) : base(bus)
	    {
	    }

	    public override async Task<CreateUserResponse> HandleCommand(CreateUser message)
	    {
		    await Task.Delay(100);

			var response = new CreateUserResponse {UserId = Guid.NewGuid()};

		    return response;
	    }
    }
}
