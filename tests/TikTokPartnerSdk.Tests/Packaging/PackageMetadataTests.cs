using System.Xml.Linq;
using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Packaging;

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
        xml.Should().Contain("<PackageLicenseExpression>");
        xml.Should().Contain("<RepositoryUrl>");
        xml.Should().NotContain("example.invalid");
        xml.Should().Contain("<PackageReadmeFile>");
        xml.Should().Contain("<GenerateDocumentationFile>true</GenerateDocumentationFile>");
        xml.Should().Contain("<Version>0.1.0-preview</Version>");
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
