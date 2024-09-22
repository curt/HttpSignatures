using System;

namespace SevenKilo.HttpSignatures;

public static class SignatureComposer
{
    public static Result Compose(SignatureModel signatureModel, out string? signature)
    {
        signature = $"keyId=\"{signatureModel.KeyId}\","
            + $"headers=\"{string.Join(' ', signatureModel.Headers)}\","
            + $"signature=\"{signatureModel.SignatureHash}\"";

        return new Result();
    }
}
