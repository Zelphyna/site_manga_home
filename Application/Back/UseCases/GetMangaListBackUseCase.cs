using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Application.Back.UseCases;

public sealed class GetMangaListBackUseCase(IMangaRepository repository)
{
    public IReadOnlyList<Manga> Execute() => repository.GetAll();
}
