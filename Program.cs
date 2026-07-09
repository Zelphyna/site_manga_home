var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<site_manga_home.Infrastructure.MangaRepository>();
builder.Services.AddSingleton<site_manga_home.Application.Front.Interfaces.IMangaReadRepository>(
    sp => sp.GetRequiredService<site_manga_home.Infrastructure.MangaRepository>());
builder.Services.AddSingleton<site_manga_home.Application.Back.Interfaces.IMangaRepository>(
    sp => sp.GetRequiredService<site_manga_home.Infrastructure.MangaRepository>());

builder.Services.AddScoped<site_manga_home.Application.Front.UseCases.GetMangaListUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.GetMangaListBackUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.GetMangaByIdUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.SaveMangaUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.DeleteMangaUseCase>();
builder.Services.AddScoped<site_manga_home.Application.Back.UseCases.UpdateTomesPossedesUseCase>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaPageRoute("Front", "/Index", "");
    options.Conventions.AddAreaPageRoute("Back", "/Index", "back");
    options.Conventions.AddAreaPageRoute("Back", "/Mangas/Edit", "back/mangas/edit");
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
