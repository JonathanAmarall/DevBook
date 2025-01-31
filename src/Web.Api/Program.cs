using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);
builder.Services.Configure<CookiePolicyOptions>(options
    => options.MinimumSameSitePolicy = SameSiteMode.Lax);
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
// Configurar autenticação com GitHub
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie()
    .AddOAuth("GitHub", options =>
    {
        IConfigurationSection githubAuthConfig = builder.Configuration.GetSection("GitHubAuth");
        options.ClientId = githubAuthConfig["ClientId"]!;
        options.ClientSecret = githubAuthConfig["ClientSecret"]!;
        options.CallbackPath = new PathString("/api/auth/github/callback");

        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";

        options.SaveTokens = true;

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                HttpResponseMessage response = await context.Backchannel.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var user = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                // Salvar informações do usuário no ticket
                context.RunClaimActions(user.RootElement);
            }
        };
    });

WebApplication app = builder.Build();
app.UseCookiePolicy();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

// REMARK: If you want to use Controllers, you'll need this.
app.MapControllers();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public partial class Program;
}
