using System.Runtime.Serialization;

namespace Q2.Oao.Library.Common.Exceptions
{
	public class OaoAuthException : OaoException
	{
		protected OaoAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public OaoAuthException(string message) : base(message)
		{
		}
	}
}