using SevenKilo.HttpSignatures.Internal;
using System.Security.Cryptography;
using System.Text;

namespace SevenKilo.HttpSignatures;

public class Signature(IKeyProvider keyProvider)
{
    private StringPairs? _headersVerified;
    private string? _signatureComposed;

    public StringPairs? HeadersVerified => _headersVerified;
    public string? SignatureComposed => _signatureComposed;

    public async Task<Result> SignAsync(ISignatureRequest request)
    {
        var keyModel = await keyProvider.GetKeyModelByKeyIdAsync(request.KeyId);

        if (keyModel == null || !keyModel.HasPrivateKey)
        {
            return new Result($"Unable to find private key '{request.KeyId}' in provider.");
        }

        var headerPairs = request.Headers.Select(h => new StringPair(h, request.GetHeaderValue(h)));
        var payload = GetComparisonPayload(headerPairs);

        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(keyModel.PrivateKey);
        var signatureHash = Convert.ToBase64String(rsa.SignData(payload, CryptoConfig.MapNameToOID("SHA256")!));
        var signatureModel = new SignatureModel(request.KeyId, signatureHash, request.Headers);

        return SignatureComposer.Compose(signatureModel, out _signatureComposed);
    }

    public async Task<Result> VerifyAsync(IVerificationRequest request)
    {
        StringPairs headerPairs = [];

        return SignatureParser.Parse(request.Signature, out var signatureModel)
            && GetHeaderPairsFromRequest(request, signatureModel!, out headerPairs)
            && request.Preverify(signatureModel!, headerPairs!)
            && await VerifyAsync(signatureModel!, headerPairs!);
    }

    private async Task<Result> VerifyAsync(SignatureModel signatureModel, StringPairs headerPairsIn)
    {
        var headers = signatureModel.Headers;
        var paths = Helpers.TraverseKeyValuePaths(headerPairsIn, headers);
        var keyId = signatureModel.KeyId;
        var keyModel = await keyProvider.GetKeyModelByKeyIdAsync(keyId);

        if (keyModel == null)
        {
            return new Result($"Unable to find key '{keyId}' in provider.");
        }

        foreach (var path in paths)
        {
            if (VerifyPath(keyModel, signatureModel, path))
            {
                _headersVerified = path;
                return new Result();
            }
        }

        return new Result("Unable to verify signature with given headers.");
    }

    private static bool VerifyPath(KeyModel keyModel, SignatureModel signatureModel, StringPairs headers)
    {
        var signature = Convert.FromBase64String(signatureModel.SignatureHash);
        var payload = GetComparisonPayload(headers);

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

    private static byte[] GetComparisonPayload(StringPairs headerPairs)
    {
        return Encoding.UTF8.GetBytes(string.Join('\n', headerPairs.Select(h => $"{h.Key}: {h.Value}")));
    }
}
