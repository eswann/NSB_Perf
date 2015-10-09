using System;
using System.Collections.Generic;

namespace Q2.Oao.Library.Common.Messages
{
	public abstract class ResponseBase : IResponse
	{
		public List<Exception> Exceptions { get; set; } = new List<Exception>();

		public List<string> Actions { get; set; } = new List<string>();

		public bool HasErrors => Exceptions != null && Exceptions.Count > 0;
	}
}