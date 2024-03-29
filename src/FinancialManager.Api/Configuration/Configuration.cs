﻿using FinancialManager.Api.Services;
using FinancialManager.Commons.Helpers;
using FinancialManager.Data.Models;
using FinancialManager.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinancialManager.Api.Configuration;

public static class Configuration
{
    public static IServiceCollection ConfigureApi(this IServiceCollection services, WebApplicationBuilder builder)
    {
        ConfigureSecurity(services, builder);
        ConfigureServices(services);
        RegisterServices(services);
        RegisterRepositories(services);
        ConfigureMongoDb(services, builder);
        ConfigureVariables(services, builder);

        return services;
    }

    static IServiceCollection ConfigureVariables(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("nf-pwd")))
            Environment.SetEnvironmentVariable("nf-pwd", builder.Configuration["nf-pwd"]);

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("nf-email")))
            Environment.SetEnvironmentVariable("nf-email", builder.Configuration["nf-email"]);

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("nf-imei")))
            Environment.SetEnvironmentVariable("nf-imei", builder.Configuration["nf-imei"]);


        return services;
    }

    static IServiceCollection ConfigureSecurity(this IServiceCollection services, WebApplicationBuilder builder)
    {
        CryptHelper.CryptEncodingKey = builder.Configuration["CryptEncodingKey"] ?? string.Empty;
        return services;
    }

    static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers();

        var key = Encoding.ASCII.GetBytes(Secret.Random(30).ToString());
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        return services;
    }

    static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<TokenService>();
        services.AddTransient<UserService>();
        services.AddTransient<InvestmentService>();

        return services;
    }

    static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<UserRepository>();
        services.AddTransient<ProductRepository>();
        return services;
    }

    static IServiceCollection ConfigureMongoDb(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var configurationSection = builder.Configuration.GetSection("FinancialBuddyDatabase");
        var dbUser = Environment.GetEnvironmentVariable("DbUser") ?? builder.Configuration["DbUser"];
        var dbPwd = Environment.GetEnvironmentVariable("DbPWd") ?? builder.Configuration["DbPwd"];
        var serverUri = Environment.GetEnvironmentVariable("ServerUri") ?? builder.Configuration["ServerUri"];
        var connectionString = configurationSection.GetValue<string>("ConnectionString").Replace("{User}", dbUser).Replace("{Password}", dbPwd).Replace("{serverUri}", serverUri);
        var databaseName = configurationSection.GetValue<string>("DatabaseName");
        var usersCollectionName = configurationSection.GetValue<string>("UsersCollectionName");
        var productsCollectionName = configurationSection.GetValue<string>("ProductsCollectionName");

        services.AddSingleton(new FinancialBuddyDatabaseSettings(connectionString, databaseName)
        {
            UsersCollectionName = usersCollectionName,
            ProductsCollectionName = productsCollectionName
        });

        return services;
    }
}
