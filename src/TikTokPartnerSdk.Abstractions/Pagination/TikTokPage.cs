namespace Loscardos.TikTokPartnerSdk.Abstractions.Pagination;

public sealed record TikTokPage<TItem>(
    IReadOnlyList<TItem> Items,
    string NextPageToken,
    long TotalCount)
{
    public bool HasNextPage => !string.IsNullOrWhiteSpace(NextPageToken);
}
