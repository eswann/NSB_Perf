using Q2.Oao.Application.Command.Api.Contracts;
using Q2.Oao.Library.Validation;

namespace Q2.Oao.Application.Command.Api.Validation
{
	public class ApplicantStartValidator : NotNullParentValidator<StartApplicationRequest>
	{
		public ApplicantStartValidator()
		{
			RuleFor(x => x.FirstName).IsName();
			RuleFor(x => x.LastName).IsName();
			RuleFor(x => x.Email).IsEmail();
			RuleFor(x => x.Password).IsPassword();
		}

	}
}