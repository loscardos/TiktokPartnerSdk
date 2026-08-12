using System.Runtime.CompilerServices;

namespace Loscardos.TikTokPartnerSdk.Abstractions.Pagination;

public static class TikTokPagedEnumerable
{
    public static async IAsyncEnumerable<TItem> ReadAllAsync<TItem>(
        TikTokPageRequest firstPage,
        Func<TikTokPageRequest, CancellationToken, Task<TikTokPage<TItem>>> fetchPageAsync,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageRequest = firstPage;

        while (true)
        {
            var page = await fetchPageAsync(pageRequest, cancellationToken).ConfigureAwait(false);
            foreach (var item in page.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }

            if (!page.HasNextPage)
            {
                yield break;
            }

            pageRequest = pageRequest with { PageToken = page.NextPageToken };
        }
    }
}
