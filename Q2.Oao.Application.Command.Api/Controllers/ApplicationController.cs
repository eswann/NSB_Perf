using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using NServiceBus;
using Q2.Oao.Application.Command.Api.Contracts;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Application.Messages.Responses;
using Q2.Oao.Library.Api.Results;
using Q2.Oao.Library.Api.Validation;
using Q2.Oao.Library.Identity;
using Q2.Oao.Library.ServiceBus.Extensions;
using Serilog;
using StartApplicationResponse = Q2.Oao.Application.Command.Api.Contracts.StartApplicationResponse;

namespace Q2.Oao.Application.Command.Api.Controllers
{
    public class ApplicationController : Controller
    {
		private readonly IBus _bus;
		private readonly IIdentityClient _identityClient;

	    public ApplicationController(IBus bus, IIdentityClient identityClient)
		{
			_bus = bus;
			_identityClient = identityClient;
		}

	    // POST api/values
		[Route("[controller]/start")]
        [HttpPost]
		[ValidationFilter]
        public async Task<IActionResult> Post([FromBody]StartApplicationRequest request)
        {
	        var command = new StartApplicationSaga
	        {
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				Password = request.Password,
				EligibilityQualifierId = request.EligibilityQualifierId,
				ProductId = request.ProductId
			};

	        var commandResponse = await _bus.SendCommand<StartApplicationSagaResponse>(command);

			Log.Debug("Got response back from StartApplicationSaga");
			Log.Debug("Attempting to get a token from the identity server at " + _identityClient.EndpointUri);
			var authResult = await _identityClient.GetToken(request.Email, request.Password);

			Log.Debug("Got a response from the Identity Server");

			var response = new StartApplicationResponse
			{
				ApplicationId = commandResponse.ApplicationId,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				Token = authResult.AccessToken,
				EligibilityQualifierId = request.EligibilityQualifierId,
				ProductId = request.ProductId
			};

			return OaoResult.Created(response.ApplicationId, response, commandResponse.Actions);
        }

		[HttpDelete]
		[Authorize]
		public void Delete(Guid applicationId)
		{

		}
	}
}
