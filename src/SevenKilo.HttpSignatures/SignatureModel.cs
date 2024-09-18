namespace SevenKilo.HttpSignatures;

public class SignatureModel
{
    public SignatureModel() { }

    public SignatureModel(string keyId, string signatureHash, IEnumerable<string> headers)
    {
        KeyId = keyId;
        SignatureHash = signatureHash;
        Headers = headers;
    }

    public string? KeyId { get; set; }
    public string? SignatureHash { get; set; }
    public IEnumerable<string> Headers { get; set; } = [];
}
