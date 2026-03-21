using AvatarSDK;
using Xunit;
using Xunit.Abstractions;

namespace AvatarSDK_Tests;

public class ApiTests(ITestOutputHelper output)
{
    private readonly API _api = new("AvatarSDK-IntegrationTests/1.0");

    // ── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsResults()
    {
        var results = await _api.SearchAsync("test");
        Assert.NotEmpty(results);
        output.WriteLine($"SearchAsync('test') → {results.Count} results");
    }

    [Theory]
    [InlineData(Platform.Windows)]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public async Task SearchAsync_EachPlatform_ReturnsResults(Platform platform)
    {
        var results = await _api.SearchAsync("avatar", platform);
        Assert.NotEmpty(results);
        output.WriteLine($"SearchAsync platform={platform} → {results.Count} results");
    }

    [Fact]
    public async Task SearchAsync_ResultsHaveRequiredFields()
    {
        var results = await _api.SearchAsync("test");
        foreach (var doc in results)
        {
            Assert.False(string.IsNullOrWhiteSpace(doc.Id),   $"Avatar missing Id");
            Assert.False(string.IsNullOrWhiteSpace(doc.Name), $"Avatar {doc.Id} missing Name");
        }
    }

    [Fact]
    public async Task SearchAsync_WithMarketplaceFlag_DoesNotThrow()
    {
        var opts    = new SearchOptions { Marketplace = true };
        var results = await _api.SearchAsync("avatar", options: opts);
        Assert.NotNull(results);
        output.WriteLine($"SearchAsync(marketplace=true) → {results.Count} results");
    }

    [Fact]
    public async Task SearchAsync_SortNewest_DoesNotThrow()
    {
        var opts    = new SearchOptions { SortMode = SortMode.Newest };
        var results = await _api.SearchAsync("avatar", options: opts);
        Assert.NotNull(results);
        output.WriteLine($"SearchAsync(sort=Newest) → {results.Count} results");
    }

    // ── GetAvatarAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetAvatarAsync_KnownId_ReturnsAvatar()
    {
        var search = await _api.SearchAsync("test");
        Assert.NotEmpty(search);

        var id     = search[0].Id;
        var avatar = await _api.GetAvatarAsync(id);

        Assert.NotNull(avatar);
        Assert.Equal(id, avatar.Id);
        output.WriteLine($"GetAvatarAsync({id}) → {avatar.Name}");
    }

    [Fact]
    public async Task GetAvatarAsync_UnknownId_ReturnsNull()
    {
        var result = await _api.GetAvatarAsync("avtr_this_does_not_exist_00000000");
        Assert.Null(result);
    }

    // ── GetRandomAsync ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(Platform.Windows)]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public async Task GetRandomAsync_EachPlatform_ReturnsResults(Platform platform)
    {
        var results = await _api.GetRandomAsync(platform);
        Assert.NotEmpty(results);
        output.WriteLine($"GetRandomAsync({platform}) → {results.Count} results, first={results[0].Id}");
    }

    [Fact]
    public async Task GetRandomAsync_TwoCallsReturnDifferentResults()
    {
        var first  = await _api.GetRandomAsync();
        var second = await _api.GetRandomAsync();

        var firstIds  = first .Select(a => a.Id).ToHashSet();
        var secondIds = second.Select(a => a.Id).ToHashSet();
        output.WriteLine($"Random overlap: {firstIds.Intersect(secondIds).Count()}/{firstIds.Count}");
        Assert.NotEqual(firstIds, secondIds);
    }

    // ── GetLatestAsync ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(Platform.Windows)]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public async Task GetLatestAsync_EachPlatform_ReturnsResults(Platform platform)
    {
        var results = await _api.GetLatestAsync(platform);
        Assert.NotEmpty(results);
        output.WriteLine($"GetLatestAsync({platform}) → {results.Count} results");
    }

    
    [Fact]
    public async Task GetLatestAsync_ResultsAreOrderedNewestFirst()
    {
        var results = await _api.GetLatestAsync();
        var dates   = results.Where(d => d.ListingDate.HasValue).Select(d => d.ListingDate!.Value).ToList();

        for (int i = 1; i < dates.Count; i++)
            Assert.True(dates[i - 1] >= dates[i],
                $"Not sorted: index {i - 1} ({dates[i - 1]}) < index {i} ({dates[i]})");
    }

    // ── GetMoreByAuthorAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetMoreByAuthorAsync_ReturnsAvatarsFromSameAuthor()
    {
        var search   = await _api.SearchAsync("test");
        var authorId = search.First(a => !string.IsNullOrWhiteSpace(a.AuthorId)).AuthorId;
        var byAuthor = await _api.GetMoreByAuthorAsync(authorId);

        Assert.NotEmpty(byAuthor);
        Assert.All(byAuthor, doc => Assert.Equal(authorId, doc.AuthorId));
        output.WriteLine($"GetMoreByAuthorAsync({authorId}) → {byAuthor.Count} results");
    }

    // ── AvatarDoc shape ──────────────────────────────────────────────────────

    

    [Fact]
    public async Task AvatarDoc_PlatformsFieldIsPopulated()
    {
        var results      = await _api.SearchAsync("test");
        var withPlatforms = results.Where(a => a.Platforms is { Count: > 0 }).ToList();
        output.WriteLine($"{withPlatforms.Count}/{results.Count} avatars have Platforms populated");
        Assert.NotEmpty(withPlatforms);
    }
}
