namespace SevenKilo.HttpSignatures;

public class VerificationException : Exception
{
    public VerificationException(string message) : base(message) { }

    public VerificationException(string message, Exception exception) : base(message, exception) { }
}
