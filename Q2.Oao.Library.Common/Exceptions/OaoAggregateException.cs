using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Q2.Oao.Library.Common.Exceptions
{
	public class OaoAggregateException : AggregateException, IOaoException
	{
		protected OaoAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public OaoAggregateException(IEnumerable<Exception> innerExceptions) : base(innerExceptions)
		{
			foreach (var exception in innerExceptions)
			{
				var oaoInner = exception as IOaoException;
				if (oaoInner != null && oaoInner.Actions != null)
				{
					foreach (var action in oaoInner.Actions)
					{
						if (!Actions.Contains(action))
						{
							Actions.Add(action);
						}
					}
				}
			}
		}

		public List<string> Actions { get; set; } = new List<string>();

	}
}