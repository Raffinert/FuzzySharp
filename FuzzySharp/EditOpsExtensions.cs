using Raffinert.FuzzySharp.Edits;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp;

public static class EditOpsExtensions
{
    public static List<OpCode> AsOpCodes(this IEnumerable<EditOp> editOps, int srcLen, int destLen)
    {
        var opcodes = new List<OpCode>();
        int prevI = 0, prevJ = 0;
        foreach (var op in editOps)
        {
            int i = op.SourcePos;
            int j = op.DestPos;
            // equal segment
            if (prevI < i && prevJ < j)
                opcodes.Add(new OpCode { EditType = EditType.KEEP, SourceBegin = prevI, SourceEnd = i, DestBegin = prevJ, DestEnd = j });
            // delete
            if (op.EditType == EditType.DELETE)
            {
                opcodes.Add(new OpCode { EditType = EditType.DELETE, SourceBegin = i, SourceEnd = i + 1, DestBegin = j, DestEnd = j });
                prevI = i + 1;
                prevJ = j;
            }
            // insert
            else if (op.EditType == EditType.INSERT)
            {
                opcodes.Add(new OpCode { EditType = EditType.INSERT, SourceBegin = i, SourceEnd = i, DestBegin = j, DestEnd = j + 1 });
                prevI = i;
                prevJ = j + 1;
            }
        }

        // final equal segment
        if (prevI < srcLen && prevJ < destLen)
        {
            opcodes.Add(new OpCode
            {
                EditType = EditType.KEEP,
                SourceBegin = prevI,
                SourceEnd = srcLen,
                DestBegin = prevJ,
                DestEnd = destLen
            });
        }

        return opcodes;
    }

    public static List<MatchingBlock> AsMatchingBlocks(
        this IEnumerable<EditOp> ops,
        int srcLen,
        int destLen)
    {
        var blocks = new List<MatchingBlock>();
        int srcPos = 0;
        int destPos = 0;

        foreach (var op in ops)
        {
            // emit any "skipped" matching region before this op
            if (srcPos < op.SourcePos || destPos < op.DestPos)
            {
                int length = Math.Min(op.SourcePos - srcPos,
                    op.DestPos - destPos);
                if (length > 0)
                {
                    blocks.Add(new MatchingBlock
                    {
                        SourcePos = srcPos,
                        DestPos = destPos,
                        Length = length
                    });
                }

                srcPos = op.SourcePos;
                destPos = op.DestPos;
            }

            // consume the op
            switch (op.EditType)
            {
                case EditType.REPLACE:
                    srcPos++;
                    destPos++;
                    break;
                case EditType.DELETE:
                    srcPos++;
                    break;
                case EditType.INSERT:
                    destPos++;
                    break;
            }
        }

        // any trailing match after the last op
        if (srcPos < srcLen || destPos < destLen)
        {
            int length = Math.Min(srcLen - srcPos,
                destLen - destPos);
            if (length > 0)
            {
                blocks.Add(new MatchingBlock
                {
                    SourcePos = srcPos,
                    DestPos = destPos,
                    Length = length
                });
            }
        }

        // sentinel: zero-length block at the very end
        blocks.Add(new MatchingBlock
        {
            SourcePos = srcLen,
            DestPos = destLen,
            Length = 0
        });

        return blocks;
    }
}