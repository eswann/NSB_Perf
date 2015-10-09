using System;
using System.Net;
using Microsoft.AspNet.Mvc;
using NServiceBus;
using Microsoft.AspNet.Authorization;
using Q2.Oao.Application.Command.Api.Contracts;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Library.Api.Results;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Q2.Oao.Application.Command.Api.Controllers
{
    [Route("application/{applicationId}/submit")]
    public class ApplicationSubmitController : Controller
    {
		private readonly IBus _bus;

		public ApplicationSubmitController(IBus bus)
		{
			_bus = bus;
		}

		// POST api/values
		[HttpPost]
		[Authorize]
		public IActionResult Post(Guid applicationId, [FromBody] SubmitApplicationRequest action)
        {
			var command = new SubmitApplication
			{
				CorrelationId = Guid.NewGuid(),
				ApplicationId = applicationId,
				Comment = action.Comment
			};

			_bus.Send(command);

			return OaoResult.Accepted();
		}
    }
}
