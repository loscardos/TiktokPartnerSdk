using System.Xml.Linq;
using FluentAssertions;

namespace Loscardos.TikTokPartnerSdk.Tests.Packaging;

public sealed class PackageMetadataTests
{
    [Fact]
    public void Publishable_projects_should_have_required_nuget_metadata()
    {
        var props = XDocument.Load(Path.Combine(TestPaths.RepositoryRoot, "Directory.Build.props"));
        var xml = props.ToString();

        xml.Should().Contain("<Authors>");
        xml.Should().Contain("<Company>Loscardos</Company>");
        xml.Should().Contain("<PackageId>Loscardos.$(MSBuildProjectName)</PackageId>");
        xml.Should().Contain("<AssemblyName>Loscardos.$(MSBuildProjectName)</AssemblyName>");
        xml.Should().Contain("<RootNamespace>Loscardos.$(MSBuildProjectName)</RootNamespace>");
        xml.Should().Contain("<PackageLicenseExpression>");
        xml.Should().Contain("<RepositoryUrl>");
        xml.Should().NotContain("example.invalid");
        xml.Should().Contain("<PackageReadmeFile>");
        xml.Should().Contain("<GenerateDocumentationFile>true</GenerateDocumentationFile>");
        xml.Should().Contain("<Version>0.1.0-preview</Version>");
    }

    [Fact]
    public void Shipping_source_and_publication_workflow_should_use_loscardos_identity()
    {
        var shippingSource = Directory
            .EnumerateFiles(Path.Combine(TestPaths.RepositoryRoot, "src"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains("TikTokPartnerSdk.Generator", StringComparison.Ordinal))
            .Select(File.ReadAllText)
            .ToArray();

        shippingSource.Should().NotContain(source =>
            source.Contains("namespace TikTokPartnerSdk.", StringComparison.Ordinal) ||
            source.Contains("using TikTokPartnerSdk.", StringComparison.Ordinal));
        shippingSource.Should().Contain(source =>
            source.Contains("namespace Loscardos.TikTokPartnerSdk.", StringComparison.Ordinal));

        var packageWorkflow = File.ReadAllText(Path.Combine(
            TestPaths.RepositoryRoot,
            ".github",
            "workflows",
            "package.yml"));
        packageWorkflow.Should().Contain("branches:");
        packageWorkflow.Should().Contain("- production");
        packageWorkflow.Should().Contain("packages: write");
        packageWorkflow.Should().Contain("nuget.pkg.github.com/loscardos/index.json");
        packageWorkflow.Should().NotContain("dotnet test");
        packageWorkflow.Should().NotContain("tags:");
    }

    [Fact]
    public void Sample_and_generator_should_not_be_packable_by_default()
    {
        var sample = File.ReadAllText(Path.Combine(
            TestPaths.RepositoryRoot,
            "samples",
            "TikTokPartnerSdk.SampleConsole",
            "TikTokPartnerSdk.SampleConsole.csproj"));
        var generator = File.ReadAllText(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generator",
            "TikTokPartnerSdk.Generator.csproj"));

        sample.Should().Contain("<IsPackable>false</IsPackable>");
        generator.Should().Contain("<IsPackable>false</IsPackable>");
    }
}
