using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.UseCases;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Back.Pages.Mangas;

public sealed class EditModel(
    GetMangaByIdUseCase getMangaByIdUseCase,
    SaveMangaUseCase saveMangaUseCase) : PageModel
{
    [BindProperty]
    public Manga Manga { get; set; } = new();

    public IActionResult OnGet(int? id)
    {
        if (id.HasValue)
        {
            var manga = getMangaByIdUseCase.Execute(id.Value);
            if (manga is null) return NotFound();
            Manga = manga;
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        saveMangaUseCase.Execute(Manga);
        return RedirectToPage("/Index", new { area = "Back" });
    }
}
