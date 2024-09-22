using SevenKilo.HttpSignatures.Internal;
using System.Security.Cryptography;
using System.Text;

namespace SevenKilo.HttpSignatures;

public class Signature(IKeyProvider keyProvider)
{
    public string Sign(IHeaderValueProvider headers)
    {
        throw new NotImplementedException();
    }

    public Result Verify(IVerificationRequest request, out StringPairs? headersVerified)
    {
        headersVerified = null;
        StringPairs headerPairs = [];

        return SignatureParser.Parse(request.Signature, out var signatureModel)
            && GetHeaderPairsFromRequest(request, signatureModel!, out headerPairs)
            && request.Preverify(signatureModel!, headerPairs!)
            && Verify(signatureModel!, headerPairs!, out headersVerified);
    }

    private Result Verify(SignatureModel signatureModel, StringPairs headerPairsIn, out StringPairs? headersVerified)
    {
        headersVerified = null;

        var headers = signatureModel.Headers;
        var paths = Helpers.TraverseKeyValuePaths(headerPairsIn, headers);
        var keyId = signatureModel.KeyId;
        var keyModel = keyProvider.Get(keyId);

        if (keyModel == null)
        {
            return new Result($"Unable to find key '{keyId}' in provider.");
        }

        foreach (var path in paths)
        {
            if (VerifyPath(keyModel, signatureModel, path))
            {
                headersVerified = path;
                return new Result();
            }
        }

        return new Result("Unable to verify signature with given headers.");
    }

    private static bool VerifyPath(KeyModel keyModel, SignatureModel signatureModel, StringPairs headers)
    {
        var comparisonString = string.Join('\n', headers.Select(h => $"{h.Key}: {h.Value}"));
        var signature = Convert.FromBase64String(signatureModel.SignatureHash);
        var payload = Encoding.UTF8.GetBytes(comparisonString);
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(keyModel.PublicKey);

        return rsa.VerifyData(payload, CryptoConfig.MapNameToOID("SHA256")!, signature);
    }

    private static Result GetHeaderPairsFromRequest(IVerificationRequest request, SignatureModel signatureModel, out StringPairs headerPairs)
    {
        var keys = signatureModel!.Headers;
        headerPairs = keys.SelectMany(k => request.GetHeaderValues(k).Select(v => new StringPair(k, v)));

        return new Result();
    }
}
