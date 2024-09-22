namespace SevenKilo.HttpSignatures;

public class KeyModel(string publicKey, string? privateKey)
{
    public string PublicKey { get; } = publicKey;
    public string? PrivateKey { get; } = privateKey;
    public bool HasPrivateKey { get => PrivateKey != null; }
}
