namespace Loscardos.TikTokPartnerSdk.Abstractions.Pagination;

public sealed record TikTokPageRequest(long PageSize = 50, string PageToken = "");
