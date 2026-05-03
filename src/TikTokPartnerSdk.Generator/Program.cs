namespace TikTokPartnerSdk.Generator;

public static class Program
{
    public static int Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0] : "summary";
        var schemaDirectory = args.Length > 1
            ? args[1]
            : Path.Combine("tests", "TikTokPartnerSdk.Tests", "Fixtures", "Schemas");
        var outputDirectory = args.Length > 2 ? args[2] : Directory.GetCurrentDirectory();

        var reader = new SchemaReader();
        var endpoints = reader.ReadDirectory(schemaDirectory);

        switch (mode)
        {
            case "summary":
                Console.WriteLine(new ContractWriter().WriteSummary(endpoints));
                return 0;
            case "coverage":
                Directory.CreateDirectory(outputDirectory);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "endpoint-coverage.md"),
                    new EndpointCoverageWriter().WriteMarkdown(endpoints));
                return 0;
            case "generate":
                Directory.CreateDirectory(outputDirectory);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "endpoint-coverage.md"),
                    new EndpointCoverageWriter().WriteMarkdown(endpoints));

                foreach (var group in endpoints.GroupBy(static endpoint => endpoint.ModuleKey).OrderBy(static group => group.Key, StringComparer.Ordinal))
                {
                    var modulePascal = TikTokName.ToPascalCase(group.Key);
                    var generatedDirectory = Path.Combine(outputDirectory, "src", "TikTokPartnerSdk.Generated", modulePascal);
                    var abstractionsDirectory = Path.Combine(outputDirectory, "src", "TikTokPartnerSdk.Abstractions", "Managers", "Generated");
                    var coreDirectory = Path.Combine(outputDirectory, "src", "TikTokPartnerSdk.Core", "Managers", "Generated");

                    Directory.CreateDirectory(generatedDirectory);
                    Directory.CreateDirectory(abstractionsDirectory);
                    Directory.CreateDirectory(coreDirectory);

                    File.WriteAllText(
                        Path.Combine(generatedDirectory, modulePascal + "Contracts.g.cs"),
                        new ContractsWriter().WriteModule(group.First().ModuleName, group.Key, group.ToArray()));

                    File.WriteAllText(
                        Path.Combine(abstractionsDirectory, TikTokName.ToManagerInterfaceName(group.Key) + ".g.cs"),
                        new ManagersWriter().WriteInterface(group.First().ModuleName, group.Key, group.ToArray()));

                    File.WriteAllText(
                        Path.Combine(coreDirectory, TikTokName.ToManagerImplementationName(group.Key) + ".g.cs"),
                        new ManagersWriter().WriteImplementation(group.First().ModuleName, group.Key, group.ToArray()));
                }

                return 0;
            default:
                Console.Error.WriteLine("Usage: dotnet run --project src/TikTokPartnerSdk.Generator -- [summary|coverage|generate] [schemaDirectory] [outputDirectory]");
                return 2;
        }
    }
}
