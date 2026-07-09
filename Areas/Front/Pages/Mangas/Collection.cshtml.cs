using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Areas.Front.Pages.Mangas;

public sealed class CollectionModel(IMangaRepository repository) : PageModel
{
    public Manga Manga { get; private set; } = new();

    public HashSet<int> OwnedTomes { get; private set; } = [];

    [TempData]
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet(int id)
    {
        var manga = repository.GetById(id);
        if (manga is null) return NotFound();

        Manga = manga;
        OwnedTomes = manga.TomesPossedesNumeros.ToHashSet();

        return Page();
    }

    public IActionResult OnPost(int id, [FromForm] List<int>? ownedTomes)
    {
        var manga = repository.GetById(id);
        if (manga is null) return NotFound();

        var normalizedOwnedTomes = (ownedTomes ?? [])
            .Where(tome => tome >= 1 && tome <= manga.TomesTotal)
            .Distinct()
            .OrderBy(tome => tome)
            .ToList();

        manga.TomesPossedesNumeros = normalizedOwnedTomes;
        manga.TomesPossedes = normalizedOwnedTomes.Count;

        repository.Update(manga);

        SuccessMessage = "Collection mise a jour.";
        return RedirectToPage(new { id });
    }
}
