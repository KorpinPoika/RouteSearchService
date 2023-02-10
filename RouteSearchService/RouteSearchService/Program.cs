using Microsoft.AspNetCore.Mvc.Versioning;
using RouteSearchService;
using RouteSearchService.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

Config.Init(builder.Environment);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(opt => {
		opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
		opt.AssumeDefaultVersionWhenUnspecified = true;
		opt.ReportApiVersions = true;
		opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
			new HeaderApiVersionReader("x-api-version"),
			new MediaTypeApiVersionReader("x-api-version"));
	}
);

builder.Services.AddHostedService<RouteCacheService>();

builder.Services.AddScoped<IRouteProvider, RouteProviderOne>();
builder.Services.AddScoped<IRouteProvider, RouteProviderTwo>();
builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();