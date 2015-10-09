using System;

namespace Q2.Oao.Library.Common.Context
{
	public class AppContext : IAppContext
	{
		public Guid CorrelationId { get; set; }

		public string TenantId { get; set; } 
	}
}