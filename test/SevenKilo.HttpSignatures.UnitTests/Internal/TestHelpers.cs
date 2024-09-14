using SevenKilo.HttpSignatures.Internal;

namespace SevenKilo.HttpSignatures.UnitTests.Internal;

[TestClass]
public class TestHelpers
{
    [TestMethod]
    public void TestTraverseKeyValuePaths()
    {
        var list = new List<KeyValuePair<string, int>>();
        list.AddRange
        ([
            new("a", 1),
            new("b", 2),
            new("b", 3),
            new("c", 4)
        ]);

        var paths = Helpers.TraverseKeyValuePaths(list, ["a", "b", "c"]);
        Assert.AreEqual(2, paths.Count());
        foreach (var path in paths)
        {
            Assert.AreEqual(3, path.Count());
        }
    }
}
