using AiBusinessWorkflow.Api.Services.AI;
using AiBusinessWorkflow.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AiBusinessWorkflow.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove existing IAiService registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAiService));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register fake AI service
            services.AddScoped<IAiService, FakeAiService>();
        });
    }
}
