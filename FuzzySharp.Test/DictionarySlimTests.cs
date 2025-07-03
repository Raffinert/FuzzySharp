using NUnit.Framework;
using Raffinert.FuzzySharp.Utils;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class DictionarySlimTests
{
    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void DictionarySlim_And_Dictionary_ShouldHaveEqualResults(string s1, string s2)
    {
        var ds1 = new DictionarySlimPooled<char, int>(64);
        var d1 = new Dictionary<char, int>(64);

        for (var index = 0; index < s1.Length; index++)
        {
            var c = s1[index];
            ref var val = ref ds1.GetOrAddValueRef(c);
            val = index + 1;
            d1[c] = index + 1;
        }

        foreach (var c in s1)
        {
            Assert.True(ds1.TryGetValue(c, out var value));
            Assert.AreEqual(value, d1[c]);
        }

        var ds2 = new DictionarySlimPooled<char, int>(64);
        var d2 = new Dictionary<char, int>(64);

        for (var index = 0; index < s2.Length; index++)
        {
            var c = s2[index];
            ref var val = ref ds2.GetOrAddValueRef(c);
            val = index + 1;

            d2[c] = index + 1;
        }

        foreach (var c in s2)
        {
            Assert.True(ds2.TryGetValue(c, out var value));
            Assert.AreEqual(value, d2[c]);
        }
    }
}
