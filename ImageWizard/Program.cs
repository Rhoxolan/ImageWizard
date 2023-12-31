using ImageWizard.Data.Contexts;
using ImageWizard.Services.ImagesServices.GetImageUrlService;
using ImageWizard.Services.ImagesServices.ImagesFileWorkerService;
using ImageWizard.Services.ImagesServices.SaveImageService;
using ImageWizard.Services.ImagesServices.UploadFromUrlImageService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ImagesContext>(opt
    => opt.UseSqlServer(builder.Configuration.GetConnectionString("ImagesDB")));

builder.Services.AddControllers();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(builder =>
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer()
    .AddHttpClient()
    .AddSwaggerGen()
    .AddTransient<IUploadFromUrlImageService, UploadFromUrlImageService>()
    .AddSingleton<IImagesFileWorkerService, ImagesFileWorkerService>()
    .AddTransient<IImageService, ImageService>()
    .AddTransient<IGetImageUrlService, GetImageUrlService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
