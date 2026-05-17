using System;

namespace Raffinert.FuzzySharp.PreProcess;

internal static class StringPreprocessorFactory
{
   public static Func<string, string> GetPreprocessor(PreprocessMode mode)
    {
        return mode switch
        {
            PreprocessMode.Full => StringPreprocessor.Full,
            PreprocessMode.None => StringPreprocessor.None,
            _ => throw new InvalidOperationException($"Invalid string preprocessor mode: {mode}")
        };
    }
}