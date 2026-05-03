using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class GeneratorProgramTests
{
    [Fact]
    public void Generator_should_emit_coverage_markdown_for_fixture_directory()
    {
        var schemaDirectory = Path.Combine(
            TestPaths.RepositoryRoot,
            "tests",
            "TikTokPartnerSdk.Tests",
            "Fixtures",
            "Schemas");
        var outputDirectory = Path.Combine(
            TestPaths.RepositoryRoot,
            "artifacts",
            "generator-tests");

        Directory.CreateDirectory(outputDirectory);

        var exitCode = TikTokPartnerSdk.Generator.Program.Main(
        [
            "coverage",
            schemaDirectory,
            outputDirectory
        ]);

        exitCode.Should().Be(0);
        File.Exists(Path.Combine(outputDirectory, "endpoint-coverage.md")).Should().BeTrue();
    }

    [Fact]
    public void Coverage_output_should_include_auth_metadata()
    {
        var schemaDirectory = Path.Combine(
            TestPaths.RepositoryRoot,
            "tests",
            "TikTokPartnerSdk.Tests",
            "Fixtures",
            "Schemas");
        var outputDirectory = Path.Combine(
            TestPaths.RepositoryRoot,
            "artifacts",
            "generator-tests");

        Directory.CreateDirectory(outputDirectory);

        var exitCode = TikTokPartnerSdk.Generator.Program.Main(
        [
            "coverage",
            schemaDirectory,
            outputDirectory
        ]);

        exitCode.Should().Be(0);

        var coverageFile = Path.Combine(outputDirectory, "endpoint-coverage.md");
        File.ReadAllText(coverageFile).Should().Contain("access_token_kind=partner");
        File.ReadAllText(coverageFile).Should().Contain("required_headers=x-tts-access-token,content-type");
    }
}
