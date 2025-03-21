using CibusServer.DAL.SQL;
using CibusServer.Interfaces;
using CibusServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
JwtService.secret = builder.Configuration["secret"];
JwtService.expDate = builder.Configuration["expirationInMinutes"];
builder.Services.AddSingleton<JwtService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            
            var revocationService = context.HttpContext.RequestServices.GetRequiredService<JwtService>();
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (token != null && !revocationService.IsTokenExist(token))
                {
                    context.Fail("This token has been revoked.");
                }

            }
            
            
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddSingleton<RepositoryBase, Repository>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IMessageService, MessageService>();

builder.Host.UseSerilog(((context, services) =>
{
    var config = new ConfigurationBuilder()
       .AddEnvironmentVariables()
       .Build();
    var template = "{Timestamp:yyyy-MM-dd HH:mm:ss}  {Level:u4}  {Message:lj}{NewLine}{Exception}";
    services
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: template)
        .WriteTo.Debug(outputTemplate: template)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .ReadFrom.Configuration(context.Configuration);

    var logFolder = context.Configuration["LOG_FOLDER"] ?? "Log";
    var logFile = Path.Combine(logFolder, "DALService.log");
    services.WriteTo.File(logFile, outputTemplate: template, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, rollOnFileSizeLimit: true, fileSizeLimitBytes: 10000000);
}));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cibus Server",
        Version = "v1",
        Description = "Cibus Server - REST API",
        Contact = new OpenApiContact
        {
            Name = "Oron",
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
    });
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.MapControllers();

app.Run();
