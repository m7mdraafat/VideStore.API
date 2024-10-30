using VideStore.Api;
using VideStore.Api.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerMiddleWare();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
