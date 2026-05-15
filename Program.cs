using Amazon.S3;
using Amazon.S3.Model;
using DotNetEnv;
using MongoDB.Driver;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;
using simple_artifacterp_back.Services;
using simple_artifacterp_back.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Env.Load();

var mongoUser = Environment.GetEnvironmentVariable("USER_DB");
var mongoPassword = Environment.GetEnvironmentVariable("PASSWORD_DB");
var mongoPort = Environment.GetEnvironmentVariable("PORT_DB");
var mongoHost = Environment.GetEnvironmentVariable("HOST_DB") ?? "localhost";
var escapedUser = Uri.EscapeDataString(mongoUser ?? string.Empty);
var escapedPassword = Uri.EscapeDataString(mongoPassword ?? string.Empty);
var mongoConnectionString = $"mongodb://{escapedUser}:{escapedPassword}@{mongoHost}:{mongoPort}";
var mongoDatabaseName = "simple_artifacterp";

builder.Services.AddSingleton(new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<MongoClient>().GetDatabase(mongoDatabaseName));

var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY_BUCKET");
var secretKey = Environment.GetEnvironmentVariable("SCRET_KEY_BUCKET");
var bucketEndpoint = Environment.GetEnvironmentVariable("ENDPOINT_BUCKET");
var bucketName = Environment.GetEnvironmentVariable("NAME_BUCKET");

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = bucketEndpoint,
        ForcePathStyle = true
    };

    return new AmazonS3Client(accessKey, secretKey, config);
});
builder.Services.AddSingleton(new S3BucketSettings
{
    BucketName = bucketName ?? string.Empty,
    ServiceUrl = bucketEndpoint ?? string.Empty
});

builder.Configuration["Jwt:Key"] ??= Environment.GetEnvironmentVariable("JWT_KEY") ?? "dev-secret-key";
builder.Configuration["Jwt:Issuer"] ??= Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "simple-artifacterp";
builder.Configuration["Jwt:Audience"] ??= Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "simple-artifacterp";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<AssetsRepository>();
builder.Services.AddScoped<SuppliesRepository>();
builder.Services.AddScoped<UnitsMeasurementRepository>();
builder.Services.AddScoped<AssetsCatalogService>();
builder.Services.AddScoped<SuppliesCatalogService>();
builder.Services.AddScoped<UnitsMeasurementCatalogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
