using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.UseCases;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Back.Pages;

public sealed class IndexModel(
    GetMangaListBackUseCase getMangaListUseCase,
    DeleteMangaUseCase deleteMangaUseCase) : PageModel
{
    public IReadOnlyList<Manga> Mangas { get; private set; } = [];

    public void OnGet()
    {
        Mangas = getMangaListUseCase.Execute();
    }

    public IActionResult OnPostDelete(int id)
    {
        deleteMangaUseCase.Execute(id);
        return RedirectToPage();
    }
}
