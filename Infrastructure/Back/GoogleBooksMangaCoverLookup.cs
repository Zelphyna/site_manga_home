using System.Text.Json;
using site_manga_home.Application.Back.Interfaces;

namespace site_manga_home.Infrastructure.Back;

public sealed class GoogleBooksMangaCoverLookup(HttpClient httpClient) : IMangaCoverLookup
{
    public async Task<string?> FindTomeOneCoverUrlAsync(string titre, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titre)) return null;

        var coverFromGoogleBooks = await FindFromGoogleBooksAsync(titre, cancellationToken);
        if (!string.IsNullOrWhiteSpace(coverFromGoogleBooks))
            return coverFromGoogleBooks;

        var coverFromOpenLibrary = await FindFromOpenLibraryAsync(titre, cancellationToken);
        if (!string.IsNullOrWhiteSpace(coverFromOpenLibrary))
            return coverFromOpenLibrary;

        return null;
    }

    private async Task<string?> FindFromGoogleBooksAsync(string titre, CancellationToken cancellationToken)
    {
        var query = Uri.EscapeDataString($"intitle:{titre} manga tome 1");
        var requestUri = $"volumes?q={query}&printType=books&maxResults=8&langRestrict=fr";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!json.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
            return null;

        string? fallback = null;

        foreach (var item in items.EnumerateArray())
        {
            if (!item.TryGetProperty("volumeInfo", out var volumeInfo))
                continue;

            if (!TryGetImageUrl(volumeInfo, out var imageUrl))
                continue;

            if (fallback is null)
                fallback = imageUrl;

            var title = GetString(volumeInfo, "title");
            if (LooksLikeTomeOne(title))
                return imageUrl;

            if (volumeInfo.TryGetProperty("subtitle", out var subtitleElement))
            {
                var subtitle = subtitleElement.GetString();
                if (LooksLikeTomeOne(subtitle))
                    return imageUrl;
            }
        }

        return fallback;
    }

    private async Task<string?> FindFromOpenLibraryAsync(string titre, CancellationToken cancellationToken)
    {
        var query = Uri.EscapeDataString(titre);
        var requestUri = $"https://openlibrary.org/search.json?title={query}&language=fre&limit=12";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!json.RootElement.TryGetProperty("docs", out var docs) || docs.ValueKind != JsonValueKind.Array)
            return null;

        string? fallback = null;

        foreach (var doc in docs.EnumerateArray())
        {
            if (!doc.TryGetProperty("cover_i", out var coverIdElement) || coverIdElement.ValueKind != JsonValueKind.Number)
                continue;

            if (!coverIdElement.TryGetInt32(out var coverId) || coverId <= 0)
                continue;

            var imageUrl = $"https://covers.openlibrary.org/b/id/{coverId}-L.jpg";

            if (fallback is null)
                fallback = imageUrl;

            var title = GetString(doc, "title");
            if (LooksLikeTomeOne(title))
                return imageUrl;
        }

        return fallback;
    }

    private static bool TryGetImageUrl(JsonElement volumeInfo, out string imageUrl)
    {
        imageUrl = string.Empty;

        if (!volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
            return false;

        var thumbnail = GetString(imageLinks, "thumbnail") ?? GetString(imageLinks, "smallThumbnail");
        if (string.IsNullOrWhiteSpace(thumbnail))
            return false;

        imageUrl = NormalizeImageUrl(thumbnail);
        return true;
    }

    private static string NormalizeImageUrl(string imageUrl)
    {
        var normalized = imageUrl.Replace("http://", "https://", StringComparison.OrdinalIgnoreCase);
        return normalized.Contains("&edge=curl", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : $"{normalized}&edge=curl";
    }

    private static bool LooksLikeTomeOne(string? titleOrSubtitle)
    {
        if (string.IsNullOrWhiteSpace(titleOrSubtitle)) return false;
        var normalized = titleOrSubtitle.Trim().ToLowerInvariant();

        return normalized.Contains("tome 1")
            || normalized.Contains("vol. 1")
            || normalized.Contains("volume 1")
            || normalized.EndsWith(" 1")
            || normalized.Contains("#1");
    }

    private static string? GetString(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var prop) ? prop.GetString() : null;
}