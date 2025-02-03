using VideStore.Api;
using VideStore.Api.ServicesExtensions;
using VideStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerMiddleWare();

app.UseHttpsRedirection();
app.UseRouting();

app.UseStaticFiles();

app.UseCors("AllowOrigins");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHangfireDashboardAndRecurringJob(builder.Services); 

app.Run();
