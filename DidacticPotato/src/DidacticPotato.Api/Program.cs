using System.Reflection;
using DidacticPotato.Api;
using DidacticPotato.MessageBrokers.Publishers;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration;
using DidacticPotato.MessageBrokers.TestEvents;
using DidacticPotato.Serializer;
using DidacticPotato.Serializer.NewtonsoftJson.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetNewtonsoftJsonConfiguration();

builder.Services.SetRabbitMqConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/testJson", (ISerializer serializer) =>
{
    var obj = new JsonTest("test", 1, new List<string> {"test", "test2"});
    string result = serializer.ToJson(obj);
    return Results.Ok(result);
});

app.MapPost("/sendMessage", (MessageSent message, IBusPublisher busPublisher) =>
{
    busPublisher.PublishAsync(message, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
    return Results.NoContent();
});

app.Run();


namespace DidacticPotato.Api
{
    record JsonTest(string fieldTest, int field2, List<string> field3, int? field4 = null);
}