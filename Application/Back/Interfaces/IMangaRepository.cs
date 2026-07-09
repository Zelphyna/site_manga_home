using site_manga_home.Domain;

namespace site_manga_home.Application.Back.Interfaces;

public interface IMangaRepository
{
    IReadOnlyList<Manga> GetAll();
    Manga? GetById(int id);
    void Add(Manga manga);
    void Update(Manga manga);
    void Delete(int id);
}
