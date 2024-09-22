namespace SevenKilo.HttpSignatures;

public interface IVerificationRequest
{
    string Signature { get; }
    IEnumerable<string> GetHeaderValues(string key);
    Result Preverify(SignatureModel signatureModel, StringPairs headerPairs);
}
