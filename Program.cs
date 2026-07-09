using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Application.Back.UseCases;
using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Application.Front.UseCases;
using site_manga_home.Infrastructure.Back;
using site_manga_home.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddResponseCompression();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddScoped<MangaRepository>();
builder.Services.AddScoped<IMangaReadRepository>(sp => sp.GetRequiredService<MangaRepository>());
builder.Services.AddScoped<IMangaRepository>(sp => sp.GetRequiredService<MangaRepository>());
builder.Services.AddHttpClient<IMangaCoverLookup, GoogleBooksMangaCoverLookup>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("site_manga_home/1.0");
});
builder.Services.AddHostedService<MangaCoverWarmupHostedService>();

builder.Services.AddScoped<GetMangaListUseCase>();
builder.Services.AddScoped<GetMangaListBackUseCase>();
builder.Services.AddScoped<GetMangaByIdUseCase>();
builder.Services.AddScoped<SaveMangaUseCase>();
builder.Services.AddScoped<UpdateTomesPossedesUseCase>();
builder.Services.AddScoped<DeleteMangaUseCase>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseResponseCompression();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

