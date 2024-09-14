namespace SevenKilo.HttpSignatures;

public class SignatureModel(string keyId, string signatureHash, IEnumerable<string> headers)
{
    public string KeyId { get; set; } = keyId;
    public string SignatureHash { get; set; } = signatureHash;
    public IEnumerable<string> Headers { get; set; } = headers;
}
