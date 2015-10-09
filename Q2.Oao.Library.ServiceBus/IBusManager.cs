using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus;

namespace Q2.Oao.Library.ServiceBus
{
	public interface IBusManager
	{
		IBus ServiceBus { get; set; }
		IBus Start(IEnumerable<Assembly> assembliesToScan, Action<BusConfiguration> configurationAction = null);
		void Stop();
	}
}