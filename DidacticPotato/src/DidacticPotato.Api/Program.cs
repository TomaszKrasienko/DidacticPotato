using System;
using DidacticPotato.Api.Configuration;
using DidacticPotato.Api.Events;
using DidacticPotato.Api.MongoDocuments;
using DidacticPotato.MessageBrokers.Publishers;
using DidacticPotato.Persistence.MongoDB.Configuration;
using DidacticPotato.Persistence.MongoDB.Repositories;
using DidacticPotato.Serializer.NewtonsoftJson.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetNewtonsoftJsonConfiguration();
builder.Services.SetConfiguration(builder.Configuration);

var app = builder.Build();

// var subscriber = app.Services.GetRequiredService<IBusSubscriber>();
// subscriber.Subscribe<MessageReceived>(async (ServiceProvider, @event, _) =>
// {
//     using var scope = ServiceProvider.CreateScope();
//     await scope.ServiceProvider.GetRequiredService<IMessageReceiveHandler>().Handle(@event);
// });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Mongo example
#region MongoDb
app.MapPost("/add-to-mongo", async (TemporaryDocument temporaryObject,
        IMongoRepository<TemporaryDocument, Guid> repository) =>
{
    var id = Guid.NewGuid();
    await repository.AddAsync(temporaryObject with {Id = id});
    return Results.CreatedAtRoute("GetById", new {id = id}, null);
});

app.MapGet("/get-from-mongo/{id:guid}", async (Guid id, IMongoRepository<TemporaryDocument, Guid> repository) =>
{
    var result = await repository.GetByIdAsync(id);
    return Results.Ok(result);
}).WithName("GetById");

app.MapGet("/get-from-mongo", async (IMongoRepository<TemporaryDocument, Guid> repository) =>
{
    var results = await repository.FindAsync(_ => true);
    return Results.Ok(results);
});
#endregion

//RabbitMQ Example
app.MapPost("send-broker-message", async (MessageSent @event, IBusPublisher busPublisher) =>
{
    var messageId = Guid.NewGuid().ToString("N");
    var correlationId = Guid.NewGuid().ToString("N");
    await busPublisher.PublishAsync<MessageSent>(@event, messageId, correlationId);
    return Results.NoContent();
});

app.Run();