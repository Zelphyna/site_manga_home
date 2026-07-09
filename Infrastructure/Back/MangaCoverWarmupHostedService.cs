using Microsoft.Extensions.Hosting;
using site_manga_home.Application.Back.Interfaces;

namespace site_manga_home.Infrastructure.Back;

public sealed class MangaCoverWarmupHostedService(
    IServiceProvider serviceProvider,
    ILogger<MangaCoverWarmupHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IMangaRepository>();
        var mangaCoverLookup = scope.ServiceProvider.GetRequiredService<IMangaCoverLookup>();

        var mangas = repository.GetAll();

        foreach (var manga in mangas)
        {
            if (string.IsNullOrWhiteSpace(manga.Titre))
                continue;

            if (!NeedsOnlineLookup(manga.CoverUrl))
                continue;

            try
            {
                var foundCoverUrl = await mangaCoverLookup.FindTomeOneCoverUrlAsync(manga.Titre, cancellationToken);
                if (string.IsNullOrWhiteSpace(foundCoverUrl))
                    continue;

                manga.CoverUrl = foundCoverUrl;
                repository.Update(manga);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Impossible de recuperer une couverture distante pour {Titre}", manga.Titre);
            }

            // Délai pour éviter de surcharger les APIs externes
            await Task.Delay(250, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static bool NeedsOnlineLookup(string? coverUrl)
    {
        if (string.IsNullOrWhiteSpace(coverUrl))
            return true;

        return coverUrl.Contains("placehold.co", StringComparison.OrdinalIgnoreCase)
            || coverUrl.Contains("placeholder", StringComparison.OrdinalIgnoreCase);
    }
}