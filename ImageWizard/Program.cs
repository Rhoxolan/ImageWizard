using ImageWizard.Data.Contexts;
using ImageWizard.Services.ImagesServices.UploadFromUrlImageService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ImagesContext>(opt
	=> opt.UseSqlServer(builder.Configuration.GetConnectionString("ImagesDB")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer()
	.AddHttpClient()
	.AddSwaggerGen()
	.AddTransient<IUploadFromUrlImageService, UploadFromUrlImageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();