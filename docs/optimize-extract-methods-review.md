# Code Review: optimize-extract-methods Branch

**Date:** 2026-07-01  
**Branch:** `optimize-extract-methods` → `master`  
**Diff:** +1,206 / -30 lines across 7 files

---

## Summary

Refactored `ResultExtractor` to eliminate double-iteration from LINQ `.Max()` / `.MaxN(limit).Reverse()` chains. Introduced single-pass extraction with dedicated core methods and a min-heap for top-N selection.

**Files changed:**
| File | Change |
|---|---|
| `FuzzySharp\Extractor\ResultExtractor.cs` | Refactored — replaced LINQ Max/MaxN with `ExtractOneCore`, `ExtractTopCore` |
| `FuzzySharp\Extractor\ScoredCandidate.cs` | **New** — `ScoredCandidate<T>`, `BestCandidate<T>` for single-best tracking |
| `FuzzySharp\Extractor\ResultExtractor.Parallel.cs` | Parallel extractors use `BestCandidate<T>` with lock-based merging |
| `FuzzySharp\Extractor\ResultExtractor.Cached.cs` | Cached variant refactoring |
| `FuzzySharp\Extractor\ResultExtractor.Parallel.Cached.cs` | Cached parallel variant |

---

## Findings

### 🔴 HIGH — Empty sequence throws on no-match (`ResultExtractor.cs:38`, `.Parallel.cs:51`)

```csharp
return Enumerable.Empty<ExtractedResult<T>>().Max();
```

When no candidates meet the cutoff, this calls `.Max()` on an empty `IEnumerable` and throws `InvalidOperationException`. This is a **pre-existing bug** (old code did the same), so functionally unchanged — but should be addressed in a follow-up. Consider returning a default or throwing with a clearer message.

### 🟡 MEDIUM — Tie-breaking inconsistency (`ScoredCandidate.cs:18-20`)

```csharp
public override int Compare(ScoredCandidate<T> x, ScoredCandidate<T> y)
{
    return x.Score.CompareTo(y.Score);  // only compares score!
}
```

When two candidates have equal scores, the comparer returns `0` without considering index. The old `.MaxN(limit)` preserved original order for ties; the heap-based approach may return arbitrary candidates. For deterministic results:

```csharp
int cmp = x.Score.CompareTo(y.Score);
if (cmp != 0) return cmp;
return x.Index.CompareTo(y.Index);
```

### 🟡 MEDIUM — Lock contention in parallel ExtractOne (`ResultExtractor.Parallel.cs:43-46`)

Every thread that finds a qualifying candidate acquires the lock. For large datasets with many candidates above cutoff, this serializes all merges. Consider merging locally first (already done), then doing one final merge at the end instead of locking per-thread.

### 🟢 LOW — Minor inefficiencies

- **Redundant variable** (`ResultExtractor.cs:51`): `var comparer = ScoredCandidateComparer<T>.Instance;` — already stored in MinHeap constructor
- **Double `.Instance` access** on lines 50-51 of the same file

---

## ✅ Good Patterns

- `BestCandidate<T>` struct with thread-local state for parallel merge is correct
- Single-pass extraction eliminates double iteration (old code: score → Max/MaxN)
- Cutoff filtering before heap insertion avoids unnecessary allocations
- `ScoreParallel` writes to distinct array indices — safe without synchronization

---

## Overall Assessment

The refactoring is sound and achieves the stated goal of eliminating double-iteration. The main concern is the tie-breaking behavior change for equal-score candidates in ExtractTop, which could produce different results than before on edge cases.
