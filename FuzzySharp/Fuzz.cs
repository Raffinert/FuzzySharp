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
    public static double Ratio(string input1, string input2)
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
    public static double Ratio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double PartialRatio(string input1, string input2)
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
    public static double PartialRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2, preprocessor);
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
    public static double TokenSortRatio(string input1, string input2)
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
    public static double TokenSortRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double PartialTokenSortRatio(string input1, string input2)
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
    public static double PartialTokenSortRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double TokenSetRatio(string input1, string input2)
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
    public static double TokenSetRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double PartialTokenSetRatio(string input1, string input2)
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
    public static double PartialTokenSetRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double TokenDifferenceRatio(string input1, string input2)
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
    public static double TokenDifferenceRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double PartialTokenDifferenceRatio(string input1, string input2)
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
    public static double PartialTokenDifferenceRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double TokenInitialismRatio(string input1, string input2)
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
    public static double TokenInitialismRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2, preprocessor);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static double PartialTokenInitialismRatio(string input1, string input2)
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
    public static double PartialTokenInitialismRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double TokenAbbreviationRatio(string input1, string input2)
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
    public static double TokenAbbreviationRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double PartialTokenAbbreviationRatio(string input1, string input2)
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
    public static double PartialTokenAbbreviationRatio(string input1, string input2, Func<string, string> preprocessor)
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
    public static double WeightedRatio(string input1, string input2)
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
    public static double WeightedRatio(string input1, string input2, Func<string, string> preprocessor)
    {
        return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2, preprocessor);
    }
    #endregion
}
