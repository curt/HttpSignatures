namespace SevenKilo.HttpSignatures.UnitTests.Fakes;

public class FakeVerificationRequest(string signature, IEnumerable<KeyValuePair<string, string>> headerValues) : IVerificationRequest
{
    public virtual string Signature { get; } = signature;

    public virtual IEnumerable<string> GetHeaderValues(string key)
    {
        return headerValues.Where(hv => hv.Key == key).Select(hv => hv.Value);
    }

    public virtual Result Preverify(SignatureModel signatureModel, IEnumerable<KeyValuePair<string, string>> headerPairs)
    {
        return new Result();
    }
}
