using System.Text.Json.Serialization;

namespace AvatarSDK;

public class AvatarDoc
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; } = "";

    [JsonPropertyName("authorId")]
    public string AuthorId { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("thumbnailImageUrl")]
    public string? ThumbnailImageUrl { get; set; }

    [JsonPropertyName("releaseStatus")]
    public string? ReleaseStatus { get; set; }

    [JsonPropertyName("hasImposters")]
    public bool? HasImposters { get; set; }

    [JsonPropertyName("performanceRating")]
    public Dictionary<string, object> PerformanceRating { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("platforms")]
    public List<string>? Platforms { get; set; }

    [JsonPropertyName("acknowledgements")]
    public string? Acknowledgements { get; set; }

    [JsonPropertyName("listingDate")]
    public DateTimeOffset? ListingDate { get; set; }

    [JsonPropertyName("searchable")]
    public bool? Searchable { get; set; }

    [JsonPropertyName("styles")]
    public Dictionary<string, string> Styles { get; set; } = new();

    [JsonPropertyName("unityPackageUrl")]
    public string? UnityPackageUrl { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonPropertyName("version")]
    public int? Version { get; set; }

    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }
}
