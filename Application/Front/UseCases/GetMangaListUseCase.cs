using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Application.Front.UseCases;

public sealed class GetMangaListUseCase(IMangaReadRepository repository)
{
    public IReadOnlyList<Manga> Execute() => repository.GetAll();
}
