using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Application.Back.UseCases;

public sealed class GetMangaByIdUseCase(IMangaRepository repository)
{
    public Manga? Execute(int id) => repository.GetById(id);
}
