using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.UseCases;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Back.Pages;

public sealed class IndexModel(
    GetMangaListBackUseCase getMangaListUseCase,
    DeleteMangaUseCase deleteMangaUseCase,
    UpdateTomesPossedesUseCase updateTomesPossedesUseCase) : PageModel
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

    public IActionResult OnPostIncrement(int id)
    {
        var newValue = updateTomesPossedesUseCase.Increment(id);
        return Content(newValue.ToString());
    }

    public IActionResult OnPostDecrement(int id)
    {
        var newValue = updateTomesPossedesUseCase.Decrement(id);
        return Content(newValue.ToString());
    }
}
