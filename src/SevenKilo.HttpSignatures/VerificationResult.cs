namespace SevenKilo.HttpSignatures;

public class VerificationResult
{
    internal VerificationResult(IEnumerable<StringPair> headerValues)
    {
        HeaderValues = headerValues;
    }

    public IEnumerable<StringPair> HeaderValues { get; }
}
