namespace SevenKilo.HttpSignatures.UnitTests;

[TestClass]
public class TestSignatureParser
{
    [TestMethod]
    [DynamicData(nameof(ValidFixtures))]
    public void TestParseValid(string fixture)
    {
        var (frontMatter, content) = YamlFrontMatterParser.Parse<SignatureModel>(File.ReadAllText(fixture));

        Assert.IsNotNull(frontMatter);
        Assert.IsNotNull(content);

        var model = SignatureParser.Parse(content);

        Assert.AreEqual(frontMatter.KeyId, model.KeyId);
        Assert.AreEqual(frontMatter.SignatureHash, model.SignatureHash);
        Assert.AreEqual(frontMatter.Headers.Count(), model.Headers.Count());
        Assert.IsTrue(frontMatter.Headers.All(h => model.Headers.Contains(h)));
        Assert.IsTrue(model.Headers.All(h => frontMatter.Headers.Contains(h)));
    }

    private static IEnumerable<string[]> ValidFixtures
    {
        get { return GetFixturesForPath("./fixtures/signatures/valid"); }
    }

    private static IEnumerable<string[]> GetFixturesForPath(string path)
    {
        foreach (var fixture in Directory.EnumerateFiles(path, "*.txt"))
        {
            yield return new string[] { fixture };
        }
    }
}
