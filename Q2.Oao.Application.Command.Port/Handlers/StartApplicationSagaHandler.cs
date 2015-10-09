using NServiceBus;
using NServiceBus.Saga;
using Q2.Oao.Application.Command.Port.Sagas;
using Q2.Oao.Application.Messages.Commands;
using Q2.Oao.Application.Messages.Responses;
using Q2.Oao.Identity.Messages.Commands;
using Q2.Oao.Identity.Messages.Responses;
using Q2.Oao.Library.Common.Messages;

namespace Q2.Oao.Application.Command.Port.Handlers
{
	public class StartApplicationSagaHandler : Saga<StartApplicationSagaData>,
										IAmStartedByMessages<StartApplicationSaga>,
										IHandleMessages<StartApplicationResponse>,
										IHandleMessages<CreateUserResponse>
	{
		public StartApplicationSagaHandler(IBus bus)
		{
			Bus = bus;
		}

		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<StartApplicationSagaData> mapper)
		{
		}

		public void Handle(StartApplicationSaga message)
		{
			var startCommand = new StartApplication
			{
				ApplicantEmail = message.Email,
				ApplicantFirstName = message.FirstName,
				ApplicantLastName = message.LastName,
				ProductId = message.ProductId,
				EligibilityQualifierId = message.EligibilityQualifierId
			};
			Bus.Send(startCommand);

			var createUserCommand = new CreateUser
			{
				Email = message.Email,
				Password = message.Password
			};
			Bus.Send(createUserCommand);
		}

		public void Handle(StartApplicationResponse message)
		{
			Data.ApplicationId = message.ApplicationId;
			Data.StartAppCompleted = true;
			ProcessResponse(message);
		}

		public void Handle(CreateUserResponse message)
		{
			Data.UserId = message.UserId;
			Data.CreateUserCompleted = true;
			ProcessResponse(message);
		}

		private void ProcessResponse(IResponse message)
		{
			if (Data.SagaCompleted || message.HasErrors)
			{
				var response = new StartApplicationSagaResponse
				{
					ApplicationId = Data.ApplicationId.GetValueOrDefault(),
					UserId = Data.UserId,
					Exceptions = message.Exceptions,
					Actions = message.Actions
				};

				ReplyToOriginator(response);

				if(Data.SagaCompleted)
					MarkAsComplete();
			}
		}
	}
}