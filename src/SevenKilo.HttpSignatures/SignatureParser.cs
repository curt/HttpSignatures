using System.Text.RegularExpressions;

namespace SevenKilo.HttpSignatures;

public static class SignatureParser
{
    // Regular expressions adapted from Mastodon:
    // See: https://github.com/mastodon/mastodon/blob/main/app/lib/signature_parser.rb
    private static readonly string s_tokenRegex = @"[0-9a-zA-Z!#$%&'*+.^_`|~-]+";
    private static readonly string s_quotedRegex = @"""([^""]|"""")*""";
    private static readonly string s_paramRegex = $@"(?<k>{s_tokenRegex})\s*=\s*((?<v>{s_tokenRegex})|(?<qv>{s_quotedRegex}))";
    private static readonly string[] s_requiredParams = ["keyId", "signature"];

    public static SignatureModel Parse(string signature)
    {
        var keyValues = GetKeyValuesFromSignature(signature);
        var missingParams = s_requiredParams.Where(p => !keyValues.ContainsKey(p));

        if (missingParams.Any())
        {
            throw new SignatureParserException($"Signature is missing parameters '{string.Join(", ", missingParams)}'.");
        }

        return new SignatureModel(keyValues["keyId"], keyValues["signature"], keyValues["headers"].Split());
    }

    private static IDictionary<string, string> GetKeyValuesFromSignature(string signature)
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
