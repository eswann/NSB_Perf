using System;

namespace Q2.Oao.Identity.Messages.Responses
{
	public class CreateUserResponse : DefaultResponse
	{
		public Guid? UserId { get; set; }
	}
}