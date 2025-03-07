using Asp.Versioning;

namespace CoreBanking.API.Bootstraping;
public static class ApplicationServiceExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        builder.Services.AddApiVersioning(
            opts => { 
                opts.ReportApiVersions = true;
                opts.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), 
                    new HeaderApiVersionReader("X-Version")
                    );
            }
        );

        return builder;
    }
}
