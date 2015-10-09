using System;
using NServiceBus;

namespace Q2.Oao.Application.Messages.Commands
{

	public class ApplicationAction : ICommand
	{
		public Guid ApplicationId { get; set; }

		public Guid CorrelationId { get; set; }

		public string Comment { get;set; }
	}
}