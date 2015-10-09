using NServiceBus.Pipeline;

namespace Q2.Oao.Library.ServiceBus.Pipeline
{

	public class IncomingAppContextRegistration : RegisterStep
	{
		public IncomingAppContextRegistration()
			: base(
				stepId: "IncomingContextManipulation",
				behavior: typeof(IncomingAppContextBehavior),
				description: "Populates incoming AppContext")
		{
			InsertBefore(WellKnownStep.LoadHandlers);
		}
	}
}
