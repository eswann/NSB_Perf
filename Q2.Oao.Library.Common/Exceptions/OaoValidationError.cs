using System;

namespace Q2.Oao.Library.Common.Exceptions
{
	[Serializable]
	public class OaoValidationError
	{
		public string ErrorCode { get; set; }

		public string Property { get; set; }

		public object AttemptedValue { get; set; }

		public string Message { get; set; }
	}
}