using SubredditTracker.API.Helpers;
using SubredditTracker.API.Hubs;
using SubredditTracker.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHostedService<SendSubredditInfoService>();
builder.Services.ConfigureRedditApiService(builder.Configuration.GetSection("Reddit"));
builder.Services.ConfigureCache();
builder.Services.ConfigureCors();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("CORSPolicy");
app.MapHub<TrackingHub>("/hub");
app.MapControllers();
app.Run();


