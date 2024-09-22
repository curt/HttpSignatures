namespace SevenKilo.HttpSignatures;

public interface IHeaderValueProvider
{
    string? GetHeaderValue(string key);
}
