using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Application.Back.UseCases;

public sealed class SaveMangaUseCase(
    IMangaRepository repository,
    IMangaCoverLookup mangaCoverLookup)
{
    public async Task ExecuteAsync(Manga manga, CancellationToken cancellationToken = default)
    {
        if (manga.TomesPossedes > manga.TomesTotal)
            manga.TomesPossedes = manga.TomesTotal;

        if (string.IsNullOrWhiteSpace(manga.CoverUrl))
        {
            var foundCoverUrl = await mangaCoverLookup.FindTomeOneCoverUrlAsync(manga.Titre, cancellationToken);
            manga.CoverUrl = foundCoverUrl ?? BuildFallbackCoverUrl(manga.Titre);
        }

        if (manga.Id == 0)
            repository.Add(manga);
        else
            repository.Update(manga);
    }

    private static string BuildFallbackCoverUrl(string titre)
    {
        var safeTitle = Uri.EscapeDataString(string.IsNullOrWhiteSpace(titre) ? "Manga" : titre.Trim());
        return $"https://placehold.co/200x280/475569/ffffff?text={safeTitle}+Tome+1";
    }
}
