namespace Raffinert.FuzzySharp;

public static class GlobalConfig
{
    public static PartialRatioAccuracy PartialRatioAccuracy
    {
        get => SimilarityRatio.Strategy.PartialRatioStrategy.Accuracy;
        set => SimilarityRatio.Strategy.PartialRatioStrategy.Accuracy = value;
    }
}

public enum PartialRatioAccuracy
{
    Strict,
    Fast
}