# PR Review: Exact-match fast path optimization

## Summary of changes

Three files modified with 73 lines added and 2 removed:

- `FuzzySharp/Indel.Static.BestSubstring.cs` (+42) — new public overload for char
- `FuzzySharp/SimilarityRatio/Strategy/ApproximateSubstringRatioStrategy.cs` (+22, -2) — IndexOf shortcut in string strategy
- `FuzzySharp/SimilarityRatio/Strategy/CachedApproximateSubstringRatioStrategy.cs` (+11) — IndexOf shortcut in cached scorer

---

## File 1: Indel.Static.BestSubstring.cs

### What was done

Added a new public overload that shadows the generic method for char inputs:

```csharp
public static IndelSubstringMatch BestSubstringMatch(
    ReadOnlySpan<char> pattern,
    ReadOnlySpan<char> text)
```

The implementation performs an early-exact-match check via `text.IndexOf(pattern)` before falling through to the existing bit-parallel recurrence.

### Issues

| # | Severity | Description |
|---|---|---|
| 1 | **Breaking** | New overload changes method resolution for callers who previously invoked `Indel.BestSubstringMatch<T>(...)` with `T = char`. Code that explicitly passed `ReadOnlySpan<char>` will now resolve to the new overload instead of the generic one. This is a breaking API change. |
| 2 | Minor | No XML documentation on the new overload. The existing generic method has full docs; this should match. |

### Recommendations

- Add XML doc matching the generic method's style and content.
- Consider whether this should be `internal` rather than public to avoid breaking external callers who relied on generic resolution. If it's only used internally, making it non-public avoids the breaking change entirely.

---

## File 2: ApproximateSubstringRatioStrategy.cs

### What was done

Added an exact-match shortcut at the string level before delegating to the generic strategy:

```csharp
if (text.IndexOf(pattern) >= 0) return 100;
```

### Issues

| # | Severity | Description |
|---|---|---|
| 3 | Performance | **Redundant check.** The generic strategy below calls `Indel.BestSubstringMatch<T>` which will ALSO perform an exact match search internally (via the new overload). When no exact match exists, we do IndexOf twice — once here, then again inside BestSubstringMatchImpl. |
| 4 | Minor optimization | No early return for identical-length strings that are equal. We fall through to the generic strategy even though both directions would find an exact match immediately. |

### Recommendations

- **Remove this shortcut** and rely on `Indel.BestSubstringMatch` to handle exact matches efficiently. The double-check adds overhead without benefit.
- Alternatively, move it after the generic call as a fallback for cases where the generic path might miss it (though that's unlikely given the new overload).

---

## File 3: CachedApproximateSubstringRatioStrategy.cs

### What was done

Added the same IndexOf shortcut in the cached scorer's `Calculate` method.

### Issues

| # | Severity | Description |
|---|---|---|
| 5 | Minor optimization | The shortcut is placed after empty-string checks but before the length-based direction logic. We check for exact match in both directions (pattern vs text and vice versa) even though only one direction matters for a score of 100. |

### Recommendations

- No action needed — this file calls `BestSubstringMatchImpl` directly rather than going through the new overload, so there's no redundant search. The shortcut is efficient here.
- Consider adding an early return when `_processedInput1.Length == processedInput2.Length && _processedInput1 == processedInput2` to avoid even the IndexOf call for identical strings.

---

## Benchmark results summary

| Text | Dataset | Before | After | Improvement | PartialRatio | After vs PartialRatio |
|---:|---|---:|---:|---:|---:|---:|
| 1024 | Exact match | 4,411 ns | **39.5 ns** | **111.7× faster** | 388.8 ns | **9.8× faster** |
| 4096 | Exact match | 17,328 ns | **118.4 ns** | **146.3× faster** | 508.9 ns | **4.3× faster** |
| 1024 | No exact match | 4,328 ns | **4,539 ns** | 4.9% slower | 17,898 ns | **3.9× faster** |
| 4096 | No exact match | 17,490 ns | **17,122 ns** | 2.1% faster | 59,556 ns | **3.5× faster** |

Allocations: Exact-match case went from 144 B to 0 B. No-exact-match remains at 144 B.

---

## Overall recommendations

1. **Fix the breaking change**: Either make the new overload `internal` or add proper XML documentation and versioning notes.
2. **Remove redundant IndexOf** in ApproximateSubstringRatioStrategy.cs — let BestSubstringMatch handle it.
3. **Add identical-string shortcut** to CachedApproximateSubstringRatioStrategy for O(1) early return on exact equality.
4. **Document the new overload** with XML comments matching existing style.

All tests pass (5,169 on both .NET 8 and .NET 10). Build succeeds on netstandard2.0.
