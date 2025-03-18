using agent_openai_banking_assistant_csharp.Configurations;
using agent_openai_banking_assistant_csharp.Interfaces;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using OpenAI;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:8081", "http://127.0.0.1");
    });
});

// Configure authentication using Azure AD
if (builder.Environment.IsDevelopment())
{
    // In development, use the fake authentication scheme.
    builder.Services.AddAuthentication("Fake")
        .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("Fake", options => { });
}
else
{
    // In production or staging, use JWT Bearer authentication via Azure AD.
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
}

builder.Services.AddAuthorization();

// Use the custom extension method to register azure services.
builder.Services.AddAzureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
