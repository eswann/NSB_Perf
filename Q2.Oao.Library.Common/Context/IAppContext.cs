using System;

namespace Q2.Oao.Library.Common.Context
{
	public interface IAppContext
	{
		Guid CorrelationId { get; }
		string TenantId { get; }
	}
}