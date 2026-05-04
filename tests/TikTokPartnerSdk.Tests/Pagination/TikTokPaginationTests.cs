using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Pagination;

namespace TikTokPartnerSdk.Tests.Pagination;

public sealed class TikTokPaginationTests
{
    [Fact]
    public void Page_should_report_next_page_when_token_is_present()
    {
        var page = new TikTokPage<string>(["a"], "next", 10);

        page.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Paged_enumerable_should_fetch_until_next_page_token_is_empty()
    {
        var requestedTokens = new List<string>();

        var items = TikTokPagedEnumerable.ReadAllAsync(
            new TikTokPageRequest(2, string.Empty),
            async (page, cancellationToken) =>
            {
                await Task.Yield();
                requestedTokens.Add(page.PageToken);
                return page.PageToken.Length == 0
                    ? new TikTokPage<string>(["a", "b"], "next", 3)
                    : new TikTokPage<string>(["c"], string.Empty, 3);
            });

        var result = new List<string>();
        await foreach (var item in items)
        {
            result.Add(item);
        }

        result.Should().Equal("a", "b", "c");
        requestedTokens.Should().Equal(string.Empty, "next");
    }
}
