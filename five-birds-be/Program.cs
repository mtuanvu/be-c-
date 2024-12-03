using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using five_birds_be.Data;
using five_birds_be.Jwt;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30)))
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
         options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                // Ánh xạ "Role" thành ClaimTypes.Role
                var roleClaim = claimsIdentity?.FindFirst("Role");
                if (roleClaim != null)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<FooterImageService>(); // Đăng ký FooterImageService
builder.Services.AddScoped<FooterService>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
                             .SelectMany(v => v.Errors)
                             .Select(e => e.ErrorMessage)
                             .ToList();
        var errorResponse = ApiResponse<string>.Failure(400, string.Join("; ", errors));
        return new BadRequestObjectResult(errorResponse);
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});


builder.WebHost.UseUrls("http://localhost:5005");
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>(); 
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();





builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseAuthentication(); 
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation V1");
    c.RoutePrefix = string.Empty; 
});
app.UseCors("AllowReactClient");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
