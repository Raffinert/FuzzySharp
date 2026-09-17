using System;
using System.Globalization;
using Raffinert.FuzzySharp.PreProcess;
using Xunit;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

[Collection("Culture-mutating tests")]
public class StringPreprocessorTests
{
    [Fact]
    public void FullProcessorIsCultureInvariant()
    {
        var invariant = RunWithCulture(CultureInfo.InvariantCulture, () => StringPreprocessor.Full("Iİiı"));
        var turkish = RunWithCulture(new CultureInfo("tr-TR"), () => StringPreprocessor.Full("Iİiı"));

        Assert.Equal(invariant, turkish);
    }

    [Fact]
    public void FullProcessorRetainsNorwegianAndUkrainianLetters()
    {
        var result = StringPreprocessor.Full(" blåbær їжак ");

        Assert.Equal("blåbær їжак", result);
    }

    private static T RunWithCulture<T>(CultureInfo culture, Func<T> action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            return action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}

[CollectionDefinition("Culture-mutating tests", DisableParallelization = true)]
public sealed class CultureMutatingTestCollection : ICollectionFixture<object>
{
}
