using ImageWizard.Data.Contexts;
using ImageWizard.Data.Entities;
using ImageWizard.Filters.ImagesFilters;
using ImageWizard.Middlewares;
using ImageWizard.Services.ImagesServices.GetImageUrlService;
using ImageWizard.Services.ImagesServices.ImagesFileWorkerService;
using ImageWizard.Services.ImagesServices.SaveImageService;
using ImageWizard.Services.ImagesServices.UploadFromUrlImageService;
using ImageWizard.Services.JWTService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ImagesContext>(opt
	=> opt.UseSqlServer(builder.Configuration.GetConnectionString("ImagesDB")));

builder.Services.AddControllers();

builder.Services.AddCors(opt =>
	opt.AddDefaultPolicy(builder =>
	builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<ImagesContext>();

builder.Services.AddAuthentication(opt =>
{
	opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
	opt.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
	};
});

builder.Services.AddEndpointsApiExplorer()
	.AddHttpClient()
	.AddSwaggerGen()
	.AddTransient<IUploadFromUrlImageService, UploadFromUrlImageService>()
	.AddSingleton<IImagesFileWorkerService, ImagesFileWorkerService>()
	.AddTransient<IImageService, ImageService>()
	.AddTransient<IGetImageUrlService, GetImageUrlService>()
	.AddTransient<IJWTService, JWTService>()
	.AddScoped<ValidImageSizeActionFilterAttribute>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseMiddleware<ImagesProcessingAccessMiddleware>();

app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
