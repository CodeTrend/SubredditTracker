using SubredditTracker.API.Helpers;
using SubredditTracker.API.Hubs;
using SubredditTracker.API.Interfaces;
using SubredditTracker.API.Models;
using SubredditTracker.API.Services;
using SubredditTracker.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHostedService<SendSubredditInfoService>();

builder.Services.Configure<UserCredentials>(builder.Configuration.GetSection("UserCredentials"));
builder.Services.ConfigureRedditApiService(builder.Configuration.GetSection("Reddit"));

builder.Services.ConfigureCache();
builder.Services.ConfigureCors();


var app = builder.Build();


// Configure the HTTP request pipeline.
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


