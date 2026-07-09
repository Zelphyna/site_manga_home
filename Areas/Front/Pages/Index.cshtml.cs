using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Application.Front.UseCases;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Front.Pages;

public sealed class IndexModel(
    GetMangaListUseCase getMangaListUseCase,
    IMangaCoverLookup mangaCoverLookup) : PageModel
{
    public IReadOnlyList<Manga> Mangas { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Mangas = getMangaListUseCase.Execute();

        foreach (var manga in Mangas)
        {
            if (string.IsNullOrWhiteSpace(manga.Titre))
                continue;

            var tomeOneCoverUrl = await mangaCoverLookup.FindTomeOneCoverUrlAsync(manga.Titre, cancellationToken);

            manga.CoverUrl = !string.IsNullOrWhiteSpace(tomeOneCoverUrl)
                ? tomeOneCoverUrl
                : BuildFallbackCoverUrl(manga.Titre);
        }
    }

    private static string BuildFallbackCoverUrl(string titre)
    {
        var safeTitle = Uri.EscapeDataString(string.IsNullOrWhiteSpace(titre) ? "Manga" : titre.Trim());
        return $"https://placehold.co/200x280/475569/ffffff?text={safeTitle}+Tome+1";
    }
}
