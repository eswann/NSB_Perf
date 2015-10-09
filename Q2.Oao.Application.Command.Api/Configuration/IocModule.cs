using Autofac;
using Q2.Oao.Library.Identity;

namespace Q2.Oao.Application.Command.Api.Configuration
{
    public class IocModule : Module
    {
	    protected override void Load(ContainerBuilder builder)
	    {
		    builder.RegisterAssemblyTypes(ThisAssembly, typeof(IIdentityClient).Assembly).AsImplementedInterfaces().SingleInstance();

		    builder.Register(ctx =>
		    {
			    var appSettings = ctx.Resolve<AppSettings>();
			    return appSettings.IdentityServerSettings;
		    }).AsSelf().AsImplementedInterfaces();
	    }
    }
}
