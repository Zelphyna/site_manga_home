using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Front.UseCases;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Front.Pages;

public sealed class IndexModel(GetMangaListUseCase getMangaListUseCase) : PageModel
{
    public IReadOnlyList<Manga> Mangas { get; private set; } = [];

    public void OnGet()
    {
        Mangas = getMangaListUseCase.Execute();
    }
}
