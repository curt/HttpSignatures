namespace SevenKilo.HttpSignatures;

public interface IKeyProvider
{
    KeyModel? Get(string keyId);
}
