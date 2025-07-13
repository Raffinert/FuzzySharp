using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.NewAbstraction;

public class IndelStringFuzzyScorer(string input1) : IndelFuzzyScorer<char>(input1.AsMemory());