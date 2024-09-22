using SevenKilo.YamlFrontMatter;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SevenKilo.HttpSignatures.UnitTests;

[TestClass]
public class TestSignatureParser
{
    [TestMethod]
    [DynamicData(nameof(ValidFixtures))]
    public void TestParseValid(string fixture)
    {
        var (frontMatter, content) = ParseFixture(fixture);
        var result = SignatureParser.Parse(content, out var model);

        Assert.IsNotNull(model);
        Assert.AreEqual(frontMatter.KeyId, model.KeyId);
        Assert.AreEqual(frontMatter.SignatureHash, model.SignatureHash);
        Assert.AreEqual(frontMatter.Headers.Count(), model.Headers.Count());
        Assert.IsTrue(frontMatter.Headers.All(h => model.Headers.Contains(h)));
        Assert.IsTrue(model.Headers.All(h => frontMatter.Headers.Contains(h)));
    }

    [TestMethod]
    [DynamicData(nameof(ExceptionFixtures))]
    public void TestParseException(string fixture)
    {
        var (_, content) = ParseFixture(fixture);
        var result = SignatureParser.Parse(content, out var model);

        Assert.IsNull(model);
        Assert.IsTrue(result.Errors.Any());
    }

    private static (SignatureModel, string) ParseFixture(string fixture)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var (frontMatter, content) = new YamlFrontMatterParser(deserializer).Parse<SignatureModel>(File.ReadAllText(fixture));

        Assert.IsNotNull(frontMatter);
        Assert.IsNotNull(content);

        return (frontMatter, content);
    }

    private static IEnumerable<string[]> ValidFixtures
    {
        get { return GetFixturesForPath("./fixtures/signatures/valid"); }
    }

    private static IEnumerable<string[]> ExceptionFixtures
    {
        get { return GetFixturesForPath("./fixtures/signatures/exception"); }
    }

    private static IEnumerable<string[]> GetFixturesForPath(string path)
    {
        foreach (var fixture in Directory.EnumerateFiles(path, "*.txt"))
        {
            yield return new string[] { fixture };
        }
    }
}
