using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Infrastructure;

public sealed class MangaRepository : IMangaReadRepository, IMangaRepository
{
    private static int _nextId = 4;

    private static readonly List<Manga> _mangas =
    [
        new() { Id = 1, Titre = "One Piece", CoverUrl = "https://placehold.co/200x280/e84393/white?text=One+Piece", TomesTotal = 110, TomesPossedes = 20 },
        new() { Id = 2, Titre = "Naruto", CoverUrl = "https://placehold.co/200x280/f97316/white?text=Naruto", TomesTotal = 72, TomesPossedes = 72 },
        new() { Id = 3, Titre = "Dragon Ball", CoverUrl = "https://placehold.co/200x280/a855f7/white?text=Dragon+Ball", TomesTotal = 42, TomesPossedes = 30 },
    ];

    public IReadOnlyList<Manga> GetAll() => _mangas.AsReadOnly();

    public Manga? GetById(int id) => _mangas.Find(m => m.Id == id);

    public void Add(Manga manga)
    {
        manga.Id = _nextId++;
        _mangas.Add(manga);
    }

    public void Update(Manga manga)
    {
        var existing = _mangas.Find(m => m.Id == manga.Id);
        if (existing is null) return;
        existing.Titre = manga.Titre;
        existing.CoverUrl = manga.CoverUrl;
        existing.TomesTotal = manga.TomesTotal;
        existing.TomesPossedes = manga.TomesPossedes;
    }

    public void Delete(int id) => _mangas.RemoveAll(m => m.Id == id);
}
