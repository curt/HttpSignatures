namespace SevenKilo.HttpSignatures.UnitTests.Fakes;

public class FakeKeyProvider(string? publicKey, string? privateKey) : IKeyProvider
{
    public KeyModel? Get(string keyId)
    {
        if (publicKey == null)
        {
            return null;
        }

        return new KeyModel(publicKey, privateKey);
    }

    public Task<KeyModel?> GetKeyModelByKeyIdAsync(string keyId)
    {
        if (publicKey == null)
        {
            return Task.FromResult<KeyModel?>(null);
        }

        return Task.FromResult<KeyModel?>(new KeyModel(publicKey, privateKey));
    }
}
