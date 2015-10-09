using System.Linq;
using DA = System.ComponentModel.DataAnnotations;
using ValidationException = FluentValidation.ValidationException;

namespace Q2.Oao.Library.Common.Exceptions
{
	public static class ExceptionExtensions
	{
		public static OaoValidationException ToQ2Exception(this ValidationException ex)
		{
			var q2Exception = new OaoValidationException(ex.Message);
			if (ex.Errors.SafeAny())
			{
				q2Exception.Errors =
					ex.Errors.Select(
						x =>
							new OaoValidationError
							{
								AttemptedValue = x.AttemptedValue,
								ErrorCode = x.ErrorCode,
								Message = x.ErrorMessage,
								Property = x.PropertyName
							});
			}

			return q2Exception;
		}


		public static OaoValidationException ToQ2Exception(this DA.ValidationException ex)
		{
			var q2Exception = new OaoValidationException(ex.Message)
			{
				Source = ex.Source
			};

			var error = new OaoValidationError
			{
				AttemptedValue = ex.Value,
				Message = ex.ValidationResult.ErrorMessage
			};

			if (ex.ValidationResult.MemberNames.SafeAny())
			{
				error.Property = string.Join(",", ex.ValidationResult.MemberNames);
			}

			return q2Exception;
		}

	}
}
