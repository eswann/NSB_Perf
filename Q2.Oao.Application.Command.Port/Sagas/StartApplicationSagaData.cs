using System;
using NServiceBus.Saga;

namespace Q2.Oao.Application.Command.Port.Sagas
{
    public class StartApplicationSagaData : ContainSagaData
    {
		public virtual Guid? ApplicationId { get; set; }

		public virtual Guid? UserId { get; set; }

		public virtual bool StartAppCompleted { get; set; }

		public virtual bool CreateUserCompleted { get; set; }

		public virtual bool SagaCompleted => StartAppCompleted && CreateUserCompleted;
    }
}
