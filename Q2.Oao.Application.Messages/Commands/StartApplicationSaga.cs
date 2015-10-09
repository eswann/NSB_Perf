using System;
using NServiceBus;

namespace Q2.Oao.Application.Messages.Commands
{
	public class StartApplicationSaga : ICommand
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public Guid ProductId { get; set; }

		public Guid EligibilityQualifierId { get; set; }
	}
}
