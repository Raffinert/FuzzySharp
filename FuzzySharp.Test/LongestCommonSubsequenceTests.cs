using System;
using NUnit.Framework;
using Raffinert.FuzzySharp.Edits;

namespace Raffinert.FuzzySharp.Test;

public class LongestCommonSubsequenceTests
{
    [Test]
    public void LongestCommonSubsequence_MatchingBlocks_ReturnsExpectedEditOps()
    {
        var lcsBlocks = LongestCommonSubsequence.MatchingBlocks("xasdfxxxxxxxxxxxxxxxxxxxasdfxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxasdfx".AsSpan(), "aabbcc".AsSpan());
        Assert.That(lcsBlocks, Is.EquivalentTo(new[]
        {
            new MatchingBlock
            {
                SourcePos = 1,
                Length = 1
            },
            new MatchingBlock
            {
                SourcePos = 24,
                DestPos = 1,
                Length = 1
            },
            new MatchingBlock
            {
                SourcePos = 73,
                DestPos = 6,
                Length = 0
            }}));
    }
}