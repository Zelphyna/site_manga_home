using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Infrastructure;

public sealed class MangaRepository : IMangaReadRepository, IMangaRepository
{
    private static int _nextId = 4;

    private static readonly List<Manga> _mangas =
    [
        new() { Id = 1, Titre = "One Piece", CoverUrl = "https://placehold.co/200x280/e84393/white?text=One+Piece", TomesTotal = 110, TomesPossedes = 20, TomesPossedesNumeros = BuildInitialOwnedTomes(110, 20) },
        new() { Id = 2, Titre = "Naruto", CoverUrl = "https://placehold.co/200x280/f97316/white?text=Naruto", TomesTotal = 72, TomesPossedes = 72, TomesPossedesNumeros = BuildInitialOwnedTomes(72, 72) },
        new() { Id = 3, Titre = "Dragon Ball", CoverUrl = "https://placehold.co/200x280/a855f7/white?text=Dragon+Ball", TomesTotal = 42, TomesPossedes = 30, TomesPossedesNumeros = BuildInitialOwnedTomes(42, 30) },
    ];

    public IReadOnlyList<Manga> GetAll()
    {
        foreach (var manga in _mangas)
            NormalizeOwnedTomes(manga);

        return _mangas.AsReadOnly();
    }

    public Manga? GetById(int id)
    {
        var manga = _mangas.Find(m => m.Id == id);
        if (manga is null) return null;

        NormalizeOwnedTomes(manga);
        return manga;
    }

    public void Add(Manga manga)
    {
        NormalizeOwnedTomes(manga);
        manga.Id = _nextId++;
        _mangas.Add(manga);
    }

    public void Update(Manga manga)
    {
        NormalizeOwnedTomes(manga);
        var existing = _mangas.Find(m => m.Id == manga.Id);
        if (existing is null) return;
        existing.Titre = manga.Titre;
        existing.CoverUrl = manga.CoverUrl;
        existing.TomesTotal = manga.TomesTotal;
        existing.TomesPossedes = manga.TomesPossedes;
        existing.TomesPossedesNumeros = [.. manga.TomesPossedesNumeros];
    }

    public void Delete(int id) => _mangas.RemoveAll(m => m.Id == id);

    private static void NormalizeOwnedTomes(Manga manga)
    {
        var maxTomes = Math.Max(0, manga.TomesTotal);

        manga.TomesPossedesNumeros = manga.TomesPossedesNumeros
            .Where(tome => tome >= 1 && tome <= maxTomes)
            .Distinct()
            .OrderBy(tome => tome)
            .ToList();

        manga.TomesPossedes = manga.TomesPossedesNumeros.Count;
    }

    private static List<int> BuildInitialOwnedTomes(int tomesTotal, int tomesPossedes)
    {
        var ownedCount = Math.Clamp(tomesPossedes, 0, Math.Max(0, tomesTotal));
        return Enumerable.Range(1, ownedCount).ToList();
    }
}
