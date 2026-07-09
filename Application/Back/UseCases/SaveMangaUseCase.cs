using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain;

namespace site_manga_home.Application.Back.UseCases;

public sealed class SaveMangaUseCase(IMangaRepository repository)
{
    public void Execute(Manga manga)
    {
        if (manga.TomesPossedes > manga.TomesTotal)
            manga.TomesPossedes = manga.TomesTotal;

        if (manga.Id == 0)
            repository.Add(manga);
        else
            repository.Update(manga);
    }
}
