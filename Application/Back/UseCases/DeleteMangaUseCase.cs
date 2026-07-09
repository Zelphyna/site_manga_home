using site_manga_home.Application.Back.Interfaces;

namespace site_manga_home.Application.Back.UseCases;

public sealed class DeleteMangaUseCase(IMangaRepository repository)
{
    public void Execute(int id) => repository.Delete(id);
}
