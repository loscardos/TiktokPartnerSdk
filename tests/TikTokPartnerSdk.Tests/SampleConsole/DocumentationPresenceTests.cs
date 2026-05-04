using FluentAssertions;

namespace TikTokPartnerSdk.Tests.SampleConsole;

public sealed class DocumentationPresenceTests
{
    [Fact]
    public void Readme_should_document_packages_install_auth_usage_and_verification()
    {
        var readme = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, "README.md"));

        readme.Should().Contain("Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection");
        readme.Should().Contain("Loscardos.TikTokPartnerSdk.Storage.EntityFramework");
        readme.Should().Contain("AddTikTokPartnerSdk");
        readme.Should().Contain("AddTikTokEntityFrameworkTokenStorage");
        readme.Should().Contain("BuildAuthorizationUrl");
        readme.Should().Contain("ExchangeCodeAsync");
        readme.Should().Contain("IOrderManager");
        readme.Should().Contain("certify-readonly");
        readme.Should().Contain("verify-packages.sh");
        readme.Should().Contain("docs/getting-started.md");
        readme.Should().Contain("docs/api/README.md");
    }

    [Fact]
    public void Github_style_docs_should_exist_for_core_topics_and_api_categories()
    {
        var root = TestPaths.RepositoryRoot;
        var docs = Path.Combine(root, "docs");

        File.Exists(Path.Combine(docs, "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "getting-started.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "authentication.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "token-storage.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "api", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "api", "orders.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "api", "products.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "api", "fulfillment.md")).Should().BeTrue();
        File.Exists(Path.Combine(docs, "api", "affiliate.md")).Should().BeTrue();
    }

    [Fact]
    public void Api_index_should_reference_every_generated_api_category()
    {
        var apiIndex = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, "docs", "api", "README.md"));

        foreach (var category in new[]
        {
            "Affiliate Creator",
            "Affiliate Partner",
            "Affiliate Seller",
            "Analytics",
            "Authorization",
            "Customer Engagement",
            "Customer Service",
            "Event",
            "Finance",
            "Fulfilled by TikTok",
            "Fulfillment",
            "Logistics",
            "Order",
            "Product",
            "Promotion",
            "Return and Refund",
            "Seller",
            "Supply Chain",
            "Tools"
        })
        {
            apiIndex.Should().Contain(category);
        }
    }

    [Fact]
    public void Webhook_documentation_should_be_present()
    {
        var root = TestPaths.RepositoryRoot;
        var webhooks = File.ReadAllText(Path.Combine(root, "docs", "webhooks.md"));
        var readme = File.ReadAllText(Path.Combine(root, "README.md"));

        webhooks.Should().Contain("TikTokPartnerSdk provides TikTok Shop webhook primitives");
        webhooks.Should().Contain("ITikTokWebhookParser");
        webhooks.Should().Contain("return Results.Ok()");
        readme.Should().Contain("Webhooks");
    }
}
