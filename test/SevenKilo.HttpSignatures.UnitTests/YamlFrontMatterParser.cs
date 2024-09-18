using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SevenKilo.HttpSignatures.UnitTests;

public class YamlFrontMatterParser
{
    public static (T?, string?) Parse<T>(string str)
    {
        str = str.TrimStart();
        var parts = str.Split("---\n");
        var yaml = parts.Length > 1 ? parts[1] : null;
        var content = parts.Length > 2 ? parts[2] : parts.Length == 1 ? str : null;

        T? frontMatter = default;

        if (yaml != null)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            frontMatter = deserializer.Deserialize<T>(yaml);
        }

        return (frontMatter, content);
    }
}
