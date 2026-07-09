using site_manga_home.Domain;

namespace site_manga_home.Application.Front.Interfaces;

public interface IMangaReadRepository
{
    IReadOnlyList<Manga> GetAll();
}
