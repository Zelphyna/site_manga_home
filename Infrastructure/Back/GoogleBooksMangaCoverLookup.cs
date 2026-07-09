using System.Text.Json;
using System.Text;
using site_manga_home.Application.Back.Interfaces;

namespace site_manga_home.Infrastructure.Back;

public sealed class GoogleBooksMangaCoverLookup(HttpClient httpClient) : IMangaCoverLookup, IMangaSeriesLookup
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

    public async Task<int?> FindTotalVolumesAsync(string titre, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titre)) return null;

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://graphql.anilist.co")
        {
            Content = new StringContent(BuildAniListSearchPayload(titre), Encoding.UTF8, "application/json")
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!json.RootElement.TryGetProperty("data", out var data)
            || !data.TryGetProperty("Page", out var page)
            || !page.TryGetProperty("media", out var media)
            || media.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var normalizedSearchTitle = NormalizeForComparison(titre);
        int? fallback = null;

        foreach (var item in media.EnumerateArray())
        {
            if (!item.TryGetProperty("volumes", out var volumesElement)
                || volumesElement.ValueKind != JsonValueKind.Number
                || !volumesElement.TryGetInt32(out var volumes)
                || volumes <= 0)
            {
                continue;
            }

            fallback ??= volumes;

            if (TitleMatches(item, normalizedSearchTitle))
                return volumes;
        }

        return fallback;
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
        
        // Première tentative: chercher en français
        var result = await SearchOpenLibraryAsync(query, "fre", cancellationToken);
        if (!string.IsNullOrWhiteSpace(result))
            return result;

        // Fallback: chercher tous les résultats sans restriction de langue
        return await SearchOpenLibraryAsync(query, null, cancellationToken);
    }

    private async Task<string?> SearchOpenLibraryAsync(string query, string? language, CancellationToken cancellationToken)
    {
        var requestUri = language is not null
            ? $"https://openlibrary.org/search.json?title={query}&language={language}&limit=12"
            : $"https://openlibrary.org/search.json?title={query}&limit=20";

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

    private static bool TitleMatches(JsonElement item, string normalizedSearchTitle)
    {
        if (!item.TryGetProperty("title", out var titleElement))
            return false;

        foreach (var propertyName in new[] { "romaji", "english", "native", "userPreferred" })
        {
            var candidate = GetString(titleElement, propertyName);
            if (IsMatchingTitle(candidate, normalizedSearchTitle))
                return true;
        }

        if (item.TryGetProperty("synonyms", out var synonyms) && synonyms.ValueKind == JsonValueKind.Array)
        {
            foreach (var synonym in synonyms.EnumerateArray())
            {
                if (synonym.ValueKind == JsonValueKind.String && IsMatchingTitle(synonym.GetString(), normalizedSearchTitle))
                    return true;
            }
        }

        return false;
    }

    private static bool IsMatchingTitle(string? candidate, string normalizedSearchTitle)
    {
        var normalizedCandidate = NormalizeForComparison(candidate);
        if (string.IsNullOrWhiteSpace(normalizedCandidate))
            return false;

        return normalizedCandidate.Equals(normalizedSearchTitle, StringComparison.Ordinal)
            || normalizedCandidate.Contains(normalizedSearchTitle, StringComparison.Ordinal)
            || normalizedSearchTitle.Contains(normalizedCandidate, StringComparison.Ordinal);
    }

    private static string NormalizeForComparison(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray();

        return new string(chars);
    }

    private static string BuildAniListSearchPayload(string titre)
    {
        return JsonSerializer.Serialize(new
        {
            query = "query ($search: String) { Page(perPage: 10) { media(search: $search, type: MANGA, sort: SEARCH_MATCH) { volumes title { romaji english native userPreferred } synonyms } } }",
            variables = new
            {
                search = titre
            }
        });
    }

    private static string? GetString(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var prop) ? prop.GetString() : null;
}