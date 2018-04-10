﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio.Strategy;

namespace FuzzySharp.SimilarityRatio.Algorithm.StrategySensitive
{
    public class TokenSetAlgorithm : StrategySensitiveAlgorithmBase
    {
        public TokenSetAlgorithm(IStringPreprocessor preprocessor) : base(preprocessor) { }

        public TokenSetAlgorithm() { }

        public TokenSetAlgorithm(IRatioStrategy strategy) : base(strategy)
        {
        }

        public TokenSetAlgorithm(IRatioStrategy strategy, IStringPreprocessor preprocessor) : base(strategy, preprocessor)
        {
        }

        public override int Calculate(string input1, string input2, IRatioStrategy strategy, IStringPreprocessor preprocessor)
        {
            input1 = preprocessor.Process(input1);
            input2 = preprocessor.Process(input2);

            var tokens1 = new HashSet<string>(Regex.Split(input1, @"\s+"));
            var tokens2 = new HashSet<string>(Regex.Split(input2, @"\s+"));

            var sortedIntersection = String.Join(" ", tokens1.Intersect(tokens2).OrderBy(s => s)).Trim();
            var sortedDiff1To2     = (sortedIntersection + " " + String.Join(" ", tokens1.Except(tokens2).OrderBy(s => s))).Trim();
            var sortedDiff2To1     = (sortedIntersection + " " + String.Join(" ", tokens2.Except(tokens1).OrderBy(s => s))).Trim();

            return new[]
            {
                strategy.Calculate(sortedIntersection, sortedDiff1To2),
                strategy.Calculate(sortedIntersection, sortedDiff2To1),
                strategy.Calculate(sortedDiff1To2,     sortedDiff2To1)
            }.Max();
        }
    }
}