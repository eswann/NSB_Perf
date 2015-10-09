using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Q2.Oao.Library.Common.Exceptions
{
	public abstract class OaoException : Exception, IOaoException
	{
		protected OaoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		protected OaoException(string message) : base(message)
		{
		}

		public List<string> Actions { get; set; } = new List<string>();
	}
}