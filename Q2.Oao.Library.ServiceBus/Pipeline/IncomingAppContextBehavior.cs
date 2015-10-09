using System;
using Autofac;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using Q2.Oao.Library.Common;
using Q2.Oao.Library.Common.Context;

namespace Q2.Oao.Library.ServiceBus.Pipeline
{
	public class IncomingAppContextBehavior : IBehavior<IncomingContext>
	{
		private readonly ILifetimeScope _lifetimeScope;

		public IncomingAppContextBehavior(ILifetimeScope lifetimeScope)
		{
			_lifetimeScope = lifetimeScope;
		}

		public void Invoke(IncomingContext context, Action next)
		{
			var headers = context.PhysicalMessage.Headers;

			string correlationId;
			Guid correlationGuid;
			if(!headers.TryGetValue(Constants.Headers.CorrelationId, out correlationId)
				|| !Guid.TryParse(correlationId, out correlationGuid))
				throw new ArgumentNullException(Constants.Headers.CorrelationId + " was not a valid Guid " + correlationId);

			string tenantId;
			if (!headers.TryGetValue(Constants.Headers.TenantId, out tenantId))
				throw new ArgumentNullException(Constants.Headers.TenantId + "TenantId was not present ");

			var appContext = new AppContext
			{
				CorrelationId = correlationGuid,
				TenantId = tenantId
			};

			var updater = new ContainerBuilder();
			updater.RegisterInstance(appContext).AsSelf().AsImplementedInterfaces();

			// Add the registrations to the current scope
			updater.Update(_lifetimeScope.ComponentRegistry);

			next();
		}
	}
}