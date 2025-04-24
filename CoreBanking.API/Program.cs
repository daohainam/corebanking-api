var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/run/secrets/connection_strings", optional: true);

builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapCoreBankingApi();

app.Run();
