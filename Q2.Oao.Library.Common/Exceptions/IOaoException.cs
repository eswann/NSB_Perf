using System.Collections.Generic;

namespace Q2.Oao.Library.Common.Exceptions
{
	public interface IOaoException
	{
		List<string> Actions { get; set; } 
	}
}