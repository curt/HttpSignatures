namespace SevenKilo.HttpSignatures;

public class SignatureParserException : Exception
{
    public SignatureParserException(string message) : base(message) { }
}
