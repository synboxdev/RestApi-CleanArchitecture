using Hestia.Api.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args).ConfigureSettings().ConfigureServices();
var app = builder.Build().ConfigurePostBuildApplication();

await app.RunAsync();