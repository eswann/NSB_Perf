using System.Runtime.Serialization;

namespace Q2.Oao.Library.Common.Exceptions
{
	public class OaoNotFoundException : OaoException
	{
		protected OaoNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public OaoNotFoundException(string message) : base(message)
		{
		}

	}
}