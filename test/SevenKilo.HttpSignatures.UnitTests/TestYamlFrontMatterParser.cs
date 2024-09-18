namespace SevenKilo.HttpSignatures.UnitTests;

[TestClass]
public class TestYamlFrontMatterParser
{
    [TestMethod]
    [DataRow("", false, false)]
    [DataRow("---\n---\n", false, false)]
    [DataRow("content only", false, true)]
    [DataRow("---\nfront_matter: \"only\"\n---\n", true, false)]
    [DataRow("---\nfront_matter: \"also\"\n---\ncontent", true, true)]
    public void TestValidParser(string s, bool hasFrontMatter, bool hasContent)
    {
        var (fm, c) = YamlFrontMatterParser.Parse<object>(s);

        if (hasFrontMatter)
        {
            Assert.IsNotNull(fm);
        }

        if (hasContent)
        {
            Assert.IsNotNull(c);
            Assert.IsFalse(string.IsNullOrEmpty(c));
        }
    }
}
