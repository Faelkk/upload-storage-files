using FilesStorage.Repositories;
using FilesStorage.Services;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUploadCloudService, UploadCloudService>();
builder.Services.AddScoped<IFileCloudService, FileCloudService>();
builder.Services.AddScoped<IFileLocalService, FileLocalService>();
builder.Services.AddScoped<ISaveStorageService, SaveStorageService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        Npgsql => Npgsql.EnableRetryOnFailure()
    )
);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UploadLocalFileConsumer>();
    x.AddConsumer<UploadCloudFileConsumer>();
    x.AddConsumer<DeleteLocalFileConsumer>();
    x.AddConsumer<DeleteCloudFileConsumer>();

    x.UsingRabbitMq(
        (ctx, cfg) =>
        {
            cfg.Host(
                "localhost",
                5672,
                "/",
                h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                }
            );

            cfg.ReceiveEndpoint(
                "upload-cloud-file-queue",
                e =>
                {
                    e.ConfigureConsumer<UploadCloudFileConsumer>(ctx);
                }
            );

            cfg.ReceiveEndpoint(
                "upload-local-file-queue",
                e =>
                {
                    e.ConfigureConsumer<UploadLocalFileConsumer>(ctx);
                }
            );

            cfg.ReceiveEndpoint(
                "delete-cloud-file-queue",
                e =>
                {
                    e.ConfigureConsumer<DeleteCloudFileConsumer>(ctx);
                }
            );

            cfg.ReceiveEndpoint(
                "delete-local-file-queue",
                e =>
                {
                    e.ConfigureConsumer<DeleteLocalFileConsumer>(ctx);
                }
            );
        }
    );
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760;
});

var app = builder.Build();

SeedDataBase.ApplyMigrationsOnDataBase(app.Services);

app.Use(
    async (context, next) =>
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
    }
);

app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Progam { }
