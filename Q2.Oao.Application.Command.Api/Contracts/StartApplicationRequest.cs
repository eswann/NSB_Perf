using System;

namespace Q2.Oao.Application.Command.Api.Contracts
{
	public class StartApplicationRequest
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public Guid ProductId { get; set; }

		public Guid EligibilityQualifierId { get; set; }
	}
}