using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddOpenApi();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760;
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex) when (ex.StatusCode == 413)
    {
        context.Response.StatusCode = 413;
        await context.Response.WriteAsync("Upload excedeu o limite máximo permitido.");
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
