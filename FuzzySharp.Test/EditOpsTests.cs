using Xunit;
using Raffinert.FuzzySharp.Edits;

namespace Raffinert.FuzzySharp.Test;

public class EditOpsTests
{
    [Fact]
    public void GetEditOps_KittenToSitting_ReturnsExpectedEditOps()
    {
        // Arrange
        string source = "kitten";
        string target = "sitting";

        // Act
        var ops = Levenshtein.GetEditOps(source, target);

        // Assert
        Assert.NotNull(ops);
        Assert.Equal(3, ops.Length);

        Assert.Equal(EditType.REPLACE, ops[0].EditType);
        Assert.Equal(0, ops[0].SourcePos);
        Assert.Equal(0, ops[0].DestPos);

        Assert.Equal(EditType.REPLACE, ops[1].EditType);
        Assert.Equal(4, ops[1].SourcePos);
        Assert.Equal(4, ops[1].DestPos);

        Assert.Equal(EditType.INSERT, ops[2].EditType);
        Assert.Equal(6, ops[2].SourcePos);
        Assert.Equal(6, ops[2].DestPos);
    }

    [Fact]
    public void GetEditOps_putinIsWarCriminal_ReturnsExpectedEditOps()
    {
        // Arrange
        string source = "putin";
        string target = "war criminal";

        // Act
        var ops = Levenshtein.GetEditOps(source, target);

        Assert.Equivalent(ops, new[]
        {
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 0
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 1
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 2
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 0,
                DestPos = 3
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 1,
                DestPos = 4
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 2,
                DestPos = 5
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 4,
                DestPos = 7
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 4,
                DestPos = 8
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 5,
                DestPos = 10
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 5,
                DestPos = 11
            }
        });
    }
}
