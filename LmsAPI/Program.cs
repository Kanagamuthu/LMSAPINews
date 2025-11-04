using FluentValidation;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();




#region smtp
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://192.168.0.92", "http://localhost:5050", "http://localhost/LMSAPI", "http://10.0.2.2") // your Android emulator/device IP , localhost for testing
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
        });
});
#endregion

#region Add session services
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;                 // Security best practice
    options.Cookie.IsEssential = true;              // Required for GDPR compliance
});
#endregion

#region db conn
builder.Services.AddDbContext<LmsdbNewContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region DI
builder.Services.AddScoped<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IMeUserRepository, MeUserRepository>();
builder.Services.AddScoped<LessonConverter>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});
#endregion

#region FluentValidation
builder.Services.AddScoped<IValidator<string>, EmailQueryValidator>();
builder.Services.AddScoped<IValidator<StudentRegisterDto>, StudentRegisterValidator>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<JwtTokenService>();

#endregion

#region api Versioning

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // adds API version headers in response
});

// (Optional) Add versioned API explorer (needed for Swagger)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#endregion

#region JWTToken

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Lms API v2", Version = "v2" });

    // JWT Auth support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token: **Bearer your_token_here**"
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
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();


    async Task WriteApiResponseAsync(int statusCode, string message, string errorCode = "401")
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            ErrorCode = errorCode
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

 
    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
    {
        try
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenRepo = context.RequestServices.GetRequiredService<IStudentsRepository>();

            if (!await tokenRepo.GetStudentTokenAsync(token))
            {
                await WriteApiResponseAsync(401, "Token expired or not found");
                return;
            }

          
            string tokendecoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var jwtSettings = context.RequestServices.GetRequiredService<IConfiguration>().GetSection("JwtSettings");

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
            };

            var handler = new JwtSecurityTokenHandler();
            context.User = handler.ValidateToken(tokendecoded, parameters, out _);
        }
        catch (Exception ex)
        {
            await WriteApiResponseAsync(401, "Invalid token: " + ex.Message);
            return;
        }
    }
    else if (endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null)
    {
        await WriteApiResponseAsync(401, "Authorization token missing or invalid");
        return;
    }

    await next();
});

#endregion

app.UseCors("AllowAll");
app.UseSession();//Use session middleware
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("<title>LMS API</title><h2 style='font-family: century gothic;font-size: 36px;'>Welcome to LMS API</h2>");
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
