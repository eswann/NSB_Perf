using System;

namespace Q2.Oao.Application.Messages.Responses
{
	public class StartApplicationSagaResponse : ApplicationResponse
	{
		public Guid? UserId { get; set; }
	}
}