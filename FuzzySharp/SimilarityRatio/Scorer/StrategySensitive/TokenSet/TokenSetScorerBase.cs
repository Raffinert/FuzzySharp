using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Extensions;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class TokenSetScorerBase : StrategySensitiveScorerBase
    {
        public override int Score(string input1, string input2)
        {
            var tokens1 = new HashSet<string>(input1.SplitByAnySpace());
            var tokens2 = new HashSet<string>(input2.SplitByAnySpace());

            var sortedIntersection = string.Join(" ", tokens1.Intersect(tokens2).OrderBy(s => s)).Trim();
            var sortedDiff1To2     = (sortedIntersection + " " + string.Join(" ", tokens1.Except(tokens2).OrderBy(s => s))).Trim();
            var sortedDiff2To1     = (sortedIntersection + " " + string.Join(" ", tokens2.Except(tokens1).OrderBy(s => s))).Trim();

            return new[]
            {
                Scorer(sortedIntersection, sortedDiff1To2),
                Scorer(sortedIntersection, sortedDiff2To1),
                Scorer(sortedDiff1To2,     sortedDiff2To1)
            }.Max();
        }
    }
}
