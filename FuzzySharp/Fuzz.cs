using System;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp;

public static class Fuzz
{
    #region Ratio
    /// <summary>
    /// Calculates a Levenshtein simple ratio between the strings.
    /// This indicates a measure of similarity
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int Ratio(string input1, string input2)
    {
        return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Calculates a Levenshtein simple ratio between the strings.
    /// This indicates a measure of similarity
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int Ratio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region PartialRatio
    /// <summary>
    /// Inconsistent substrings lead to problems in matching. This ratio
    /// uses a heuristic called "best partial" for when two strings
    /// are of noticeably different lengths.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Inconsistent substrings lead to problems in matching. This ratio
    /// uses a heuristic called "best partial" for when two strings
    /// are of noticeably different lengths.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region ApproximateSubstringRatio
    /// <summary>
    /// Searches the shorter input approximately within the longer input using
    /// Indel distance. Insertions and deletions cost one and a substitution
    /// costs two edits. Equal-length inputs are evaluated in both directions
    /// unless the first direction is exact. Two empty inputs score 100; exactly
    /// one empty input scores 0. This metric is not numerically equivalent to
    /// <see cref="PartialRatio(string,string)"/>.
    /// </summary>
    /// <param name="input1">The first input.</param>
    /// <param name="input2">The second input.</param>
    /// <returns>A score from 0 to 100.</returns>
    public static int ApproximateSubstringRatio(string input1, string input2)
    {
        return ScorerCache.Get<ApproximateSubstringRatioScorer>()
            .Score(input1, input2);
    }

    /// <summary>
    /// Searches the shorter processed input approximately within the longer
    /// processed input using Indel distance. Insertions and deletions cost one
    /// and a substitution costs two edits. Equal-length inputs are evaluated in
    /// both directions unless the first direction is exact. Two empty processed
    /// inputs score 100; exactly one empty processed input scores 0. This metric
    /// is not numerically equivalent to <see cref="PartialRatio(string,string)"/>.
    /// </summary>
    /// <param name="input1">The first input.</param>
    /// <param name="input2">The second input.</param>
    /// <param name="preprocessor">A preprocessor applied to both inputs.</param>
    /// <returns>A score from 0 to 100.</returns>
    public static int ApproximateSubstringRatio(
        string input1,
        string input2,
        Func<string, string> preprocessor)
    {
        return ScorerCache.Get<ApproximateSubstringRatioScorer>()
            .Score(input1, input2, preprocessor);
    }
    #endregion

    #region TokenSortRatio
    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenSortRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenSortScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int TokenSortRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenSortScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenSortRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialTokenSortRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region TokenSetRatio
    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenSetRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenSetScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int TokenSetRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenSetScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenSetRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialTokenSetRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region TokenDifferenceRatio
    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenDifferenceRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int TokenDifferenceRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenDifferenceRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialTokenDifferenceRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region TokenInitialismRatio
    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenInitialismRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int TokenInitialismRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenInitialismRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialTokenInitialismRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region TokenAbbreviationRatio
    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenAbbreviationRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int TokenAbbreviationRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenAbbreviationRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int PartialTokenAbbreviationRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2, preprocessor);
    }
    #endregion

    #region WeightedRatio
    /// <summary>
    /// Calculates a weighted ratio between the different algorithms for best results
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int WeightedRatio(string input1, string input2)
    {
        return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Calculates a weighted ratio between the different algorithms for best results
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessor"></param>
    /// <returns></returns>
    public static int WeightedRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2, preprocessor);
    }
    #endregion
}
