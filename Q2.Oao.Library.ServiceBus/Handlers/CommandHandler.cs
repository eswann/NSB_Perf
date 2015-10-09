using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using Q2.Oao.Library.Common.Exceptions;
using Q2.Oao.Library.Common.Messages;
using Serilog;
using DA = System.ComponentModel.DataAnnotations;
using ValidationException = FluentValidation.ValidationException;

namespace Q2.Oao.Library.ServiceBus.Handlers
{
	public abstract class CommandHandler<T, TResponse> : IHandleMessages<T> 
		where T:ICommand where TResponse : IResponse, new()
	{
		private const string _formatString = "Exception thrown processing command {@Command}";

		protected CommandHandler(IBus bus)
		{
			Bus = bus;
		} 

		public IBus Bus { get; set; }

		public void Handle(T command)
		{
			TResponse response;
			try
			{
				Log.Debug("Preparing to process command: {@Command}", command);

				response = HandleCommand(command).Result;
			}
			catch (OaoException ex)
			{
				response = LogExceptionAndCreateResponse(command, ex);
			}
			catch (OaoAggregateException ex)
			{
				response = LogExceptionAndCreateResponse(command, ex);
			}
			catch (ValidationException ex)
			{
				response = LogExceptionAndCreateResponse(command, ex.ToQ2Exception());
			}
			catch (DA.ValidationException ex)
			{
				response = LogExceptionAndCreateResponse(command, ex.ToQ2Exception());
			}
			catch (AggregateException ex)
			{
				if (ex.InnerExceptions.Any(x => x is DA.ValidationException || x is ValidationException || x is IOaoException))
				{
					response = new TResponse();
					foreach (var innerException in ex.InnerExceptions)
					{
						Log.Debug(ex, _formatString, command);
						if (innerException is IOaoException)
						{
							response.Exceptions.Add(innerException);
						}
						else
						{
							var daException = innerException as DA.ValidationException;
							if (daException != null)
							{
								response.Exceptions.Add(daException.ToQ2Exception());
							}
							else
							{
								var valException = innerException as ValidationException;
								if (valException != null)
									response.Exceptions.Add(valException.ToQ2Exception());
							}
						}
					}
				}
				else
				{
					Log.Error(ex, _formatString, command);
					throw;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, _formatString, command);
				throw;
			}

			Bus.Reply(response);
		}

		public abstract Task<TResponse> HandleCommand(T command);

		private static TResponse LogExceptionAndCreateResponse(T command, Exception ex)
		{
			Log.Debug(ex, _formatString, command);

			var response = new TResponse();
			response.Exceptions.Add(ex);
			return response;
		}
	}
}