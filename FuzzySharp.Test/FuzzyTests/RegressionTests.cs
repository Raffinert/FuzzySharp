using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class RegressionTests
{

    /// <summary>
    /// Test to ensure that all IRatioScorer implementations handle scoring empty strings & whitespace strings
    /// </summary>
    [Fact]
    public void TestScoringEmptyString()
    {
        var scorerType = typeof(IRatioScorer);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var types = assemblies.SelectMany(s =>
        {
            try
            {
                return s.GetTypes();
            }
            catch {}
            return [];
        }).ToList();
        var scorerTypes = types.Where(t => scorerType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass).ToList();

        string emptyString = "";
        string whitespaceString = " ";

        string[] nullOrWhitespaceStrings = [emptyString, whitespaceString];
        MethodInfo getScorerCacheMethodInfo = typeof(ScorerCache).GetMethod("Get")!;

        foreach (var t in scorerTypes)
        {
            System.Diagnostics.Debug.WriteLine($"Testing {t.Name}");
            MethodInfo m = getScorerCacheMethodInfo.MakeGenericMethod(t);
            IRatioScorer scorer = m.Invoke(this, []) as IRatioScorer;

            foreach(var s in nullOrWhitespaceStrings)
            {
                System.Diagnostics.Debug.WriteLine($"Testing string '{s}'");
                try
                {
                    scorer.Score(s, "TEST");
                }
                catch (InvalidOperationException)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as first parameter");
                }
                try
                {
                    scorer.Score("TEST", s);
                } catch (InvalidOperationException)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as second parameter");
                }
                try
                {
                    scorer.Score(s, s);
                }
                catch (InvalidOperationException)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as both parameters");
                }
            }
        }
    }
}
