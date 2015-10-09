using NServiceBus;
using System;

namespace Q2.Oao.Application.Messages.Commands
{
	public class StartApplication : ICommand
	{
		public string ApplicantFirstName { get; set; }
		public string ApplicantLastName { get; set; }
		public string ApplicantEmail { get; set; }
		public Guid ProductId { get; set; }
		public Guid EligibilityQualifierId { get; set; }
	}
}
