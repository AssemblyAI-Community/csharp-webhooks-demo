using AssemblyAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAssemblyAIClient();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.Run();