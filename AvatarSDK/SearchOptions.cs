namespace AvatarSDK;

/// <summary>Target platform for avatar queries.</summary>
public enum Platform
{
    Windows,
    Android,
    iOS,
    Web,
}

/// <summary>Controls the ordering of search results.</summary>
public enum SortMode
{
    Default = 0,
    Newest  = 1,
    Oldest  = 2,
}

/// <summary>
/// Additional filters and sort options to pass to search and random endpoints.
/// </summary>
public class SearchOptions
{
    /// <summary>Include marketplace avatars.</summary>
    public bool     Marketplace     { get; set; }

    /// <summary>Include avatars with sexual content.</summary>
    public bool     Sexual          { get; set; }

    /// <summary>Include avatars with adult themes.</summary>
    public bool     AdultThemes     { get; set; }

    /// <summary>Include avatars with graphic violence.</summary>
    public bool     GraphicViolence { get; set; }

    /// <summary>Include avatars with excessive gore.</summary>
    public bool     ExcessiveGore   { get; set; }

    /// <summary>Include avatars with extreme horror.</summary>
    public bool     ExtremeHorror   { get; set; }

    /// <summary>Primary style filter index, or <c>-1</c> for no filter.</summary>
    public int      Style1          { get; set; } = -1;

    /// <summary>Secondary style filter index, or <c>-1</c> for no filter.</summary>
    public int      Style2          { get; set; } = -1;

    /// <summary>Sort order for results.</summary>
    public SortMode SortMode        { get; set; } = SortMode.Default;

    internal int ToBitmask()
    {
        int b = 0;
        if (Marketplace)     b |= 1 << 0;
        if (Sexual)          b |= 1 << 1;
        if (AdultThemes)     b |= 1 << 2;
        if (GraphicViolence) b |= 1 << 3;
        if (ExcessiveGore)   b |= 1 << 4;
        if (ExtremeHorror)   b |= 1 << 5;
        if (Style1 >= 0)     b |= 1 << (6 + Style1);
        if (Style2 >= 0)     b |= 1 << (6 + Style2);
        b |= (int)SortMode << 16;
        return b;
    }
}
