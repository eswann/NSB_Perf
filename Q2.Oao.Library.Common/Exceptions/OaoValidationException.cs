using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Q2.Oao.Library.Common.Exceptions
{
	public class OaoValidationException : OaoException
	{
		protected OaoValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public OaoValidationException(string message) : base(message)
		{
		}

		public IEnumerable<OaoValidationError> Errors { get; set; } 

	}
}