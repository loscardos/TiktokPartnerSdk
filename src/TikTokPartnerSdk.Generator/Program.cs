using System.Text.Json;

namespace Loscardos.TikTokPartnerSdk.Generator;

public static class Program
{
    public static int Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0] : "summary";
        var schemaDirectory = args.Length > 1
            ? args[1]
            : Path.Combine("tests", "TikTokPartnerSdk.Tests", "Fixtures", "Schemas");
        var outputDirectory = args.Length > 2 ? args[2] : Directory.GetCurrentDirectory();

        switch (mode)
        {
            case "summary":
            {
                var endpoints = new SchemaReader().ReadDirectory(schemaDirectory);
                Console.WriteLine(new ContractWriter().WriteSummary(endpoints));
                return 0;
            }
            case "coverage":
            {
                var endpoints = new SchemaReader().ReadDirectory(schemaDirectory);
                Directory.CreateDirectory(outputDirectory);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "endpoint-coverage.md"),
                    new EndpointCoverageWriter().WriteMarkdown(endpoints));
                return 0;
            }
            case "generate":
            {
                var endpoints = new SchemaReader().ReadDirectory(schemaDirectory);
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
            }
            case "normalize-docs":
            {
                Directory.CreateDirectory(outputDirectory);
                var normalizer = new YamlEndpointNormalizer();
                var yamlEndpoints = new YamlApiReferenceReader().ReadDirectory(schemaDirectory);

                foreach (var endpoint in yamlEndpoints.Select(normalizer.Normalize))
                {
                    var fileName = endpoint.OperationId.Replace('_', '-') + ".json";
                    File.WriteAllText(
                        Path.Combine(outputDirectory, fileName),
                        JsonSerializer.Serialize(ToJsonShape(endpoint), new JsonSerializerOptions { WriteIndented = true }));
                }

                return 0;
            }
            default:
                Console.Error.WriteLine("Usage: dotnet run --project src/TikTokPartnerSdk.Generator -- [summary|coverage|generate|normalize-docs] [schemaDirectory] [outputDirectory]");
                return 2;
        }
    }

    private static object ToJsonShape(SchemaEndpoint endpoint)
        => new
        {
            operation_id = endpoint.OperationId,
            module_name = endpoint.ModuleName,
            module_key = endpoint.ModuleKey,
            path = endpoint.Path,
            method = endpoint.HttpMethod,
            auth_scope = endpoint.AuthScope,
            access_token_kind = endpoint.AccessTokenKind,
            request_content_kind = endpoint.RequestContentKind,
            required_headers = endpoint.RequiredHeaders,
            request_parameters = endpoint.RequestParameters.Select(ToJsonParameter).ToArray(),
            response_parameters = endpoint.ResponseParameters.Select(ToJsonParameter).ToArray()
        };

    private static object ToJsonParameter(SchemaParameter parameter)
        => new
        {
            name = parameter.Name,
            type = parameter.Type,
            required = parameter.Required,
            location = parameter.Location,
            children = parameter.Children.Select(ToJsonParameter).ToArray()
        };
}
