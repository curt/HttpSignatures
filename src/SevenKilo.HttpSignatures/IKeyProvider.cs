namespace SevenKilo.HttpSignatures;

public interface IKeyProvider
{
    Task<KeyModel?> GetKeyModelByKeyIdAsync(string keyId);
}
