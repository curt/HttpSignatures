namespace SevenKilo.HttpSignatures.UnitTests;

[TestClass]
public class TestSignatureParser
{
    [TestMethod]
    [DataRow(""""
    keyId="https://example.com/#key",headers="(request-target) host date",signature="abcdef"
    """", "https://example.com/#key", "abcdef", 3)]
    [DataRow(""""
    keyId = "https://example.com/#key" , headers ="(request-target) host date",signature ="abcdef"
    """", "https://example.com/#key", "abcdef", 3)]
    [DataRow(""""
    keyId="https://example.com/#key",headers="(request-target) host date",signature=abcdef
    """", "https://example.com/#key", "abcdef", 3)]
    [DataRow(""""
    keyId="https://example.com/#key",headers="(request-target) host date",signature="abcdef="
    """", "https://example.com/#key", "abcdef=", 3)]
    [DataRow(""""
    keyId="https://example.com/?author=alice",headers="(request-target) host date",signature="abcdef="
    """", "https://example.com/?author=alice", "abcdef=", 3)]
    [DataRow(""""
    keyId="https://example.com/?author=alice",signature="abcdef="
    """", "https://example.com/?author=alice", "abcdef=", 0)]
    public void TestParseValid(string s, string keyId, string signature, int headerCount)
    {
        var results = SignatureParser.Parse(s);

        Assert.IsNotNull(results);
        Assert.AreEqual(keyId, results.KeyId);
        Assert.AreEqual(signature, results.SignatureHash);
        Assert.AreEqual(headerCount, results.Headers.Count());
    }
}
