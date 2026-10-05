using Microsoft.EntityFrameworkCore;
using Ekub.Infrastructure.Data;
using FluentValidation;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Ekub.Application.Auth.Commands.Signup;
using Ekub.Application.Common.Behaviors;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// EF Core
var connectionString = builder.Configuration.GetConnectionString("EkubDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'EkubDatabase' was not found. Configure ConnectionStrings:EkubDatabase.");
}

builder.Services.AddDbContext<EkubDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Persistence Stores
builder.Services.AddScoped<IMemberAuthStore, MemberAuthStore>();
builder.Services.AddScoped<ICircleStore, CircleStore>();
builder.Services.AddScoped<ICircleMemberStore, CircleMemberStore>();
builder.Services.AddScoped<IPaymentStore, PaymentStore>();
builder.Services.AddScoped<IPayoutStore, PayoutStore>();
builder.Services.AddScoped<IRoundStore, RoundStore>();
builder.Services.AddScoped<IRoundLifecycleStore, RoundLifecycleStore>();

// MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(
        typeof(SignupCommand).Assembly);

    config.AddOpenBehavior(
        typeof(ValidationBehavior<,>));
});

// API Versioning
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// FluentValidation
builder.Services.AddValidatorsFromAssembly(
    typeof(SignupCommand).Assembly);

// Password hashing
builder.Services.AddScoped<
    IPasswordHasher<Member>,
    PasswordHasher<Member>>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
