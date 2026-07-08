var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<site_manga_home.Application.Front.Interfaces.IFrontLandingReadRepository, site_manga_home.Infrastructure.Front.FrontLandingReadRepository>();
builder.Services.AddScoped<site_manga_home.Application.Back.Interfaces.IBackDashboardReadRepository, site_manga_home.Infrastructure.Back.BackDashboardReadRepository>();
builder.Services.AddScoped<site_manga_home.Application.Front.UseCases.GetFrontLandingUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.GetBackDashboardUseCase>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaPageRoute("Front", "/Index", "");
    options.Conventions.AddAreaPageRoute("Back", "/Index", "back");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
