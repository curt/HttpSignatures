using SevenKilo.HttpSignatures.UnitTests.Fakes;

namespace SevenKilo.HttpSignatures.UnitTests;

[TestClass]
public class TestSignature
{
    [TestMethod]
    public async void TestSignValid()
    {
        var keyProvider = DefaultKeyProvider();
        var signatureRequest = DefaultSignatureRequest();
        var signature = new Signature(keyProvider);
        var result = await signature.SignAsync(signatureRequest);

        Assert.IsFalse(result.Errors.Any());
        Assert.IsNotNull(signature.SignatureComposed);
    }

    [TestMethod]
    public async void TestVerifyValid()
    {
        var keyProvider = DefaultKeyProvider();
        var verificationRequest = DefaultVerificationRequest();
        var signature = new Signature(keyProvider);
        var result = await signature.VerifyAsync(verificationRequest);

        Assert.IsFalse(result.Errors.Any());
        Assert.IsTrue(signature.HeadersVerified!.Any());
    }

    [TestMethod]
    public void TestVerifyMissingKeyProvider()
        => TestVerifyNotValid(MissingKeyProvider(), DefaultVerificationRequest());

    [TestMethod]
    public void TestVerifyMissingKeyVerificationRequest()
        => TestVerifyNotValid(DefaultKeyProvider(), MissingKeyVerificationRequest());

    private static async void TestVerifyNotValid(IKeyProvider keyProvider, IVerificationRequest verificationRequest)
    {
        var signature = new Signature(keyProvider);
        var result = await signature.VerifyAsync(verificationRequest);

        Assert.IsTrue(result.Errors.Any());
    }

    private static FakeKeyProvider DefaultKeyProvider()
    {
        var publicKey = File.ReadAllText("./fixtures/keys/test-public-1.txt");
        var privateKey = File.ReadAllText("./fixtures/keys/test-private-1.txt");

        return new FakeKeyProvider(publicKey, privateKey);
    }

    private static FakeKeyProvider MissingKeyProvider()
    {
        return new FakeKeyProvider(null, null);
    }

    private static FakeSignatureRequest DefaultSignatureRequest()
    {
        return new FakeSignatureRequest
        (
            [
                new("(request-target)", "post /foo?param=value&pet=dog"),
                new("host", "example.com"),
                new("date", "Sun, 05 Jan 2014 21:31:40 GMT"),
            ]
        );
    }

    private static FakeVerificationRequest DefaultVerificationRequest()
    {
        return new FakeVerificationRequest
        (
            "keyId=\"Test\",algorithm=\"rsa-sha256\","
                + "headers=\"(request-target) host date\","
                + "signature=\"qdx+H7PHHDZgy4y/Ahn9Tny9V3GP6YgBPyUXMmoxWtLbHpUnXS"
                + "2mg2+SbrQDMCJypxBLSPQR2aAjn7ndmw2iicw3HMbe8VfEdKFYRqzic+efkb3"
                + "nndiv/x1xSHDJWeSWkx3ButlYSuBskLu6kd9Fswtemr3lgdDEmn04swr2Os0=\"",
            [
                new("(request-target)", "post /foo?param=value&pet=dog"),
                new("(request-target)", "post /foo"),
                new("host", "example.com"),
                new("date", "Sun, 05 Jan 2014 21:31:40 GMT"),
            ]
        );
    }

    private static FakeVerificationRequest MissingKeyVerificationRequest()
    {
        return new FakeVerificationRequest
        (
            "headers=\"(request-target) host date\",signature=\"c2FtcGxl\"",
            [
                new("(request-target)", "target-value-b"),
                new("(request-target)", "target-value-c"),
                new("host", "host-value-d"),
                new("date", "date-value-e"),
            ]
        );
    }
}
