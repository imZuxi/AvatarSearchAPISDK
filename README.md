# AvatarSDK

A .NET SDK for interacting with the [avtr.zuxi.dev](https://avtr.zuxi.dev) avatar listing API.

> **Note:** This README and the XML doc summaries in the source were generated with AI assistance. All SDK and test code was written by hand.

---

## Installation

Add a reference to the `AvatarSDK` project or build it as a NuGet package and reference it in your project.

## Usage

```csharp
using AvatarSDK;

using var api = new API("MyApp/1.0");

// Search for avatars
List<AvatarDoc> results = await api.SearchAsync("cute fox", Platform.Windows);

// Fetch a specific avatar by ID
AvatarDoc? avatar = await api.GetAvatarAsync("avtr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");

// Get a random selection
List<AvatarDoc> random = await api.GetRandomAsync(Platform.Android);

// Get the latest listings
List<AvatarDoc> latest = await api.GetLatestAsync();

// Get all avatars by an author
List<AvatarDoc> byAuthor = await api.GetMoreByAuthorAsync("usr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
```

### Search options

`SearchOptions` lets you toggle content filters and sort order:

```csharp
var opts = new SearchOptions
{
    Marketplace = true,
    SortMode    = SortMode.Newest,
};

var results = await api.SearchAsync("avatar", Platform.Windows, opts);
```

### Uploading avatars

```csharp
// Upload by ID
await api.UploadAvatarAsync("avtr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");

// Upload with full metadata
await api.UploadAvatarAsync(new AvatarDoc { Id = "avtr_...", Name = "My Avatar" });

// Bulk upload
await api.UploadBulkAsync(listOfAvatarDocs);

// Request a metadata refresh
await api.RequestAvatarUpdateAsync("avtr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
```

## API Reference

| Method | Description |
|---|---|
| `SearchAsync(query, platform, options)` | Search avatars by keyword |
| `GetAvatarAsync(avatarId)` | Fetch a single avatar by ID |
| `GetRandomAsync(platform, options)` | Get a random selection of avatars |
| `GetLatestAsync(platform)` | Get the most recently listed avatars |
| `GetMoreByAuthorAsync(authorId)` | Get all avatars by a specific author |
| `UploadAvatarAsync(avatar)` | Upload or update an avatar |
| `UploadBulkAsync(avatars)` | Upload multiple avatars at once |
| `RequestAvatarUpdateAsync(avatarId)` | Request a metadata refresh for an avatar |

## Error handling

All methods throw `AvatarApiException` on non-success responses:

```csharp
try
{
    var avatar = await api.GetAvatarAsync("avtr_...");
}
catch (AvatarApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
```

## Running the tests

The test suite hits the live API to verify end-to-end behaviour:

```
dotnet test AvatarSDK-Tests/AvatarSDK-Tests.csproj
```
