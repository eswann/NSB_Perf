using NServiceBus;

namespace Q2.Oao.Identity.Messages.Commands
{
	public class CreateUser : ICommand
	{
		public string Email { get; set; }

		public string Password { get; set; }
	}
}