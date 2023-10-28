using DidacticPotato.Serializer;
using DidacticPotato.Serializer.NewtonsoftJson.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetNewtonsoftJsonConfiguration();

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

app.Run();


record JsonTest(string fieldTest, int field2, List<string> field3, int? field4 = null);