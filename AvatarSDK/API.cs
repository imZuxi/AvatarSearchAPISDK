using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace AvatarSDK;

/// <summary>
/// Client for the AvatarSDK REST API at <c>avtr.zuxi.dev</c>.
/// </summary>
public class API : IDisposable
{
    private const string DefaultBaseUrl = "https://avtr.zuxi.dev";
    private const string DefaultProxyBase = "https://avtr.zuxi.dev/proxy/";
    private readonly HttpClient _client;

    /// <summary>
    /// Creates a new API client.
    /// </summary>
    /// <param name="userAgent">User-Agent header sent with every request (e.g. <c>"MyApp/1.0"</c>).</param>
    /// <param name="baseUrl">Base URL of the API. Defaults to <c>https://avtr.zuxi.dev</c>.</param>
    public API(string userAgent, string baseUrl = DefaultBaseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + '/') };
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
    }


    /// <summary>
    /// Fetches a single avatar by its ID. Returns <see langword="null"/> if not found.
    /// </summary>
    public async Task<AvatarDoc> GetAvatarAsync(string avatarId)
    {
        var results = await GetAsync<List<AvatarDoc>>("search?search=" + avatarId);
        return results.FirstOrDefault();
    }

    /// <summary>
    /// Searches for avatars matching <paramref name="query"/>.
    /// </summary>
    /// <param name="query">Search term.</param>
    /// <param name="platform">Target platform filter. Defaults to <see cref="Platform.Windows"/>.</param>
    /// <param name="options">Additional search options such as content filters and sort order.</param>
    public Task<List<AvatarDoc>> SearchAsync(string query, Platform platform = Platform.Windows, SearchOptions options = default)
    {
        var bitmask = (options ?? new SearchOptions()).ToBitmask();
        return GetAsync<List<AvatarDoc>>( $"search?search={Uri.EscapeDataString(query)}&platform={ToPlatformCode(platform)}&o={bitmask}");
    }

    /// <summary>
    /// Returns a random selection of avatars.
    /// </summary>
    /// <param name="platform">Target platform filter. Defaults to <see cref="Platform.Windows"/>.</param>
    /// <param name="options">Additional search options such as content filters.</param>
    public Task<List<AvatarDoc>> GetRandomAsync(Platform? platform = Platform.Windows, SearchOptions options = default)
    {
        var bitmask = (options ?? new SearchOptions()).ToBitmask();
        return GetAsync<List<AvatarDoc>>($"random?platform={ToPlatformFull(platform.GetValueOrDefault())}&o={bitmask}");
    }

    /// <summary>
    /// Returns the most recently listed avatars.
    /// </summary>
    /// <param name="platform">Target platform filter. Defaults to <see cref="Platform.Windows"/>.</param>
    public Task<List<AvatarDoc>> GetLatestAsync(Platform? platform = Platform.Windows)
    {
        return GetAsync<List<AvatarDoc>>($"latest?platform={ToPlatformFull(platform!.Value)}");
    }

    /// <summary>
    /// Returns all avatars uploaded by the given author.
    /// </summary>
    public Task<List<AvatarDoc>> GetMoreByAuthorAsync(string authorId)
        => GetAsync<List<AvatarDoc>>($"search?authorId={Uri.EscapeDataString(authorId)}");

#region Upload Avatar
    /// <summary>
    /// Uploads or updates an avatar using a full <see cref="AvatarDoc"/>.
    /// </summary>
    public Task UploadAvatarAsync(AvatarDoc avatar)
        => PostAsync("upload-avatar", avatar);

    /// <summary>
    /// Uploads multiple avatars in a single request.
    /// </summary>
    public Task UploadBulkAsync(IEnumerable<AvatarDoc> avatars)
        => PostAsync("upload-bulk", avatars);

    /// <summary>
    /// Uploads an avatar by ID, creating a minimal record on the server.
    /// </summary>
    public Task UploadAvatarAsync(string avatarId)
        => PostAsync("upload-avatar", new AvatarDoc { Id = avatarId });

    /// <summary>
    /// Requests a metadata refresh for an existing avatar.
    /// </summary>
    public Task RequestAvatarUpdateAsync(string avatarId)
        => UploadAvatarAsync(avatarId); // @note yes this is the same as UploadAvatarByIdAsync LMAO
    #endregion


    internal string GetProxyUrl(string ImageUrl)
    {
        return ImageUrl.Replace("https://api.vrchat.cloud/api/1/image/", DefaultProxyBase)
            .Replace("https://api.vrchat.cloud/api/1/file/", DefaultProxyBase);
    }


    private async Task<T> GetAsync<T>(string relativeUrl)
    {
        var response = await _client.GetAsync(relativeUrl);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private async Task PostAsync<T>(string relativeUrl, T body)
    {
        var response = await _client.PostAsJsonAsync(relativeUrl, body);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        ApiError? error = null;
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            error = content.TrimStart().StartsWith('[')
                ? System.Text.Json.JsonSerializer.Deserialize<List<ApiError>>(content)?.FirstOrDefault()
                : System.Text.Json.JsonSerializer.Deserialize<ApiError>(content);
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException) { }

        throw new AvatarApiException(error ?? new ApiError
        {
            StatusCode = (int)response.StatusCode,
            Error = response.ReasonPhrase
        });
    }


    private static string ToPlatformCode(Platform p) => p switch
    {
        Platform.Windows           => "w",
        Platform.Android           => "a",
        Platform.iOS               => "i",
        _                          => "w"
    };

    private static string ToPlatformFull(Platform p) => p switch
    {
        Platform.Windows => "standalonewindows",
        Platform.Android           => "android",
        Platform.iOS               => "ios",
        _                          => "standalonewindows"
    };

    public void Dispose() => _client.Dispose();
}
