namespace site_manga_home.Application.Back.Interfaces;

public interface IMangaSeriesLookup
{
    Task<int?> FindTotalVolumesAsync(string titre, CancellationToken cancellationToken = default);
}