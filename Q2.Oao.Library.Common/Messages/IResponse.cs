using System;
using System.Collections.Generic;

namespace Q2.Oao.Library.Common.Messages
{
    public interface IResponse
    {
		List<Exception> Exceptions { get; set; }

		List<string> Actions { get; set; }

		bool HasErrors { get; }
	}
}
