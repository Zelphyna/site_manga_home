namespace site_manga_home.Application.Back.Interfaces;

public interface IMangaCoverLookup
{
    Task<string?> FindTomeOneCoverUrlAsync(string titre, CancellationToken cancellationToken = default);
}