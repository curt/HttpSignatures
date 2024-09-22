using System.Text.RegularExpressions;

namespace SevenKilo.HttpSignatures;

public class SignatureParser
{
    // Regular expressions adapted from Mastodon:
    // See: https://github.com/mastodon/mastodon/blob/main/app/lib/signature_parser.rb
    private static readonly string s_tokenRegex = @"[0-9a-zA-Z!#$%&'*+.^_`|~-]+";
    private static readonly string s_quotedRegex = @"""([^""]|"""")*""";
    private static readonly string s_paramRegex = $@"(?<k>{s_tokenRegex})\s*=\s*((?<v>{s_tokenRegex})|(?<qv>{s_quotedRegex}))";
    private static readonly string[] s_requiredParams = ["keyId", "signature"];

    public static Result Parse(string signature, out SignatureModel? signatureModel)
    {
        signatureModel = null;
        var keyValues = GetKeyValuesFromSignature(signature);
        var missingParams = s_requiredParams.Where(p => !keyValues.ContainsKey(p));

        if (missingParams.Any())
        {
            return new Result($"Signature is missing parameters '{string.Join(", ", missingParams)}'.");
        }

        var bytes = new Span<byte>(new byte[128]);
        if (!Convert.TryFromBase64String(keyValues["signature"], bytes, out var bytesWritten))
        {
            return new Result($"Unable to read base64 from signature hash '{keyValues["signature"]}'.");
        }

        signatureModel = new SignatureModel
        (
            keyValues["keyId"],
            keyValues["signature"],
            keyValues.TryGetValue("headers", out string? value) ? value.Split() : []
        );

        return new Result();
    }

    private static Dictionary<string, string> GetKeyValuesFromSignature(string signature)
    {
        var keyValues = new Dictionary<string, string>();
        var matches = Regex.Matches(signature, s_paramRegex);

        foreach (var m in matches)
        {
            var match = (m as Match)!;

            var key = match.Groups["k"].Value;
            var value = match.Groups["v"].Value;
            var quoted = match.Groups["qv"].Value;

            keyValues[key] = string.IsNullOrEmpty(value) ? NormalizeQuotedString(quoted) : value;
        }

        return keyValues;
    }

    private static string NormalizeQuotedString(string quotedString)
    {
        return quotedString.Trim('"').Replace("\"\"", "\"");
    }
}
