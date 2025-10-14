namespace Raffinert.FuzzySharp.Benchmarks.Docker.Utils;

public static class LevenshteinBaseline
{
    public static int GetDistance(string source, string target)
    {
        var costMatrix = Enumerable
            .Range(0, source.Length + 1)
            .Select(line => new int[target.Length + 1])
            .ToArray();

        for (var rowIndex = 1; rowIndex <= source.Length; rowIndex++)
        {
            costMatrix[rowIndex][0] = rowIndex;
        }

        for (var columnIndex = 1; columnIndex <= target.Length; columnIndex++)
        {
            costMatrix[0][columnIndex] = columnIndex;
        }

        for (var rowIndex = 1; rowIndex <= source.Length; rowIndex++)
        {
            for (var columnIndex = 1; columnIndex <= target.Length; columnIndex++)
            {
                var insertion = costMatrix[rowIndex][columnIndex - 1] + 1;
                var deletion = costMatrix[rowIndex - 1][columnIndex] + 1;
                var substitution = costMatrix[rowIndex - 1][columnIndex - 1] + (source[rowIndex - 1] == target[columnIndex - 1] ? 0 : 1);

                costMatrix[rowIndex][columnIndex] = Math.Min(Math.Min(insertion, deletion), substitution);
            }
        }

        return costMatrix[source.Length][target.Length];
    }

    public static int NewMatchEngineEditDistance(ReadOnlySpan<char> leftSpan, ReadOnlySpan<char> rightSpan)
    {
        var leftLength = leftSpan.Length;
        var rightLength = rightSpan.Length;

        if (leftLength == 0 || rightLength == 0) return 0;

        EnsureShorterFirst(ref leftSpan, ref rightSpan, ref leftLength, ref rightLength);

        var editDistance = ComputeEditDistance(leftSpan, rightSpan, leftLength, rightLength);

        return editDistance;
    }

    private static void EnsureShorterFirst(ref ReadOnlySpan<char> left, ref ReadOnlySpan<char> right, ref int leftLength, ref int rightLength)
    {
        if (leftLength <= rightLength) return;

        var tmp = left;
        left = right;
        right = tmp;

        (leftLength, rightLength) = (rightLength, leftLength);
    }

    private static int ComputeEditDistance(ReadOnlySpan<char> left, ReadOnlySpan<char> right, int leftLength, int rightLength)
    {
        const int deletionCost = 1;
        const int insertionCost = 1;
        const int substitutionCostIfMismatch = 1;
        const int substitutionCostIfMatch = 0;

        var previousRow = new int[leftLength + 1];
        var currentRow = new int[leftLength + 1];

        for (var i = 0; i <= leftLength; i++)
        {
            previousRow[i] = i;
        }

        for (var j = 1; j <= rightLength; j++)
        {
            currentRow[0] = j;
            var rightChar = right[j - 1];

            for (var i = 1; i <= leftLength; i++)
            {
                var cost = char.ToUpperInvariant(left[i - 1]) == char.ToUpperInvariant(rightChar)
                    ? substitutionCostIfMatch
                    : substitutionCostIfMismatch;

                currentRow[i] = MinOfThree(
                    previousRow[i] + deletionCost,
                    currentRow[i - 1] + insertionCost,
                    previousRow[i - 1] + cost
                );
            }

            (previousRow, currentRow) = (currentRow, previousRow);
        }
        return previousRow[leftLength];
    }

    private static int MinOfThree(int a, int b, int c)
    {
        var min = a < b ? a : b;
        return c < min ? c : min;
    }
}