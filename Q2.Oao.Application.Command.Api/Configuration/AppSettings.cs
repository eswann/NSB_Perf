using Q2.Library.Common.Configuration;
using Q2.Oao.Library.Identity.Configuration;

namespace Q2.Oao.Application.Command.Api.Configuration
{
	public class AppSettings
	{
		public LogSettings LogSettings { get; set; }

		public string CorsOrigins { get; set; } 

		public string IdentityServerAuthority { get; set; }

		public ServiceSettings ServiceSettings { get; set; }

		public IdentityServerSettings IdentityServerSettings { get; set; }

	}
}