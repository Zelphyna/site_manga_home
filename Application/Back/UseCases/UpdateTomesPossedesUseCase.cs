using site_manga_home.Application.Back.Interfaces;

namespace site_manga_home.Application.Back.UseCases;

public sealed class UpdateTomesPossedesUseCase(IMangaRepository repository)
{
    public int Increment(int id)
    {
        var manga = repository.GetById(id);
        if (manga is null) return 0;
        if (manga.TomesPossedes < manga.TomesTotal)
        {
            manga.TomesPossedes++;
            repository.Update(manga);
        }
        return manga.TomesPossedes;
    }

    public int Decrement(int id)
    {
        var manga = repository.GetById(id);
        if (manga is null) return 0;
        if (manga.TomesPossedes > 0)
        {
            manga.TomesPossedes--;
            repository.Update(manga);
        }
        return manga.TomesPossedes;
    }
}
