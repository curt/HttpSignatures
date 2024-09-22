namespace SevenKilo.HttpSignatures;

public interface ISignatureRequest
{
    string KeyId { get; }

    IEnumerable<string> Headers { get; }

    string GetHeaderValue(string key);
}
