using System;
using NServiceBus;
using Q2.Oao.Library.Common.Messages;

namespace Q2.Oao.Application.Messages.Responses
{
	public class ApplicationResponse : ResponseBase, IMessage
	{
		public Guid ApplicationId { get; set; }
	}
}