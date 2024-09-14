using System;

namespace SevenKilo.HttpSignatures.UnitTests.Fakes;

public class FakeSignatureRequest(IEnumerable<KeyValuePair<string, string>> headerPairs) : ISignatureRequest
{
    public string KeyId => "Test";

    public IEnumerable<string> Headers => headerPairs.Select(kv => kv.Key);

    public string GetHeaderValue(string key)
        => headerPairs.Where(kv => kv.Key == key).Select(kv => kv.Value).FirstOrDefault()
            ?? string.Empty;
}
