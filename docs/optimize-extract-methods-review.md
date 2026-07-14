# Code Review: optimize-extract-methods Branch

**Date:** 2026-07-01  
**Branch:** `optimize-extract-methods` → `master`  
**Diff:** +1,206 / -30 lines across 7 files

---

## Summary

Refactored `ResultExtractor` to select the best and top-N results directly, avoiding intermediate `ExtractedResult` allocations. The prior LINQ pipelines already enumerated their input once; this change reduces per-candidate overhead while preserving their selection behavior.

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

### ✅ Selection behavior is preserved

The new score-only `ScoredCandidateComparer<T>` matches the previous `ExtractedResult<T>.CompareTo` behavior used by `.MaxN(limit)`. Both implementations retain the same candidates and ordering for equal scores. `ExtractOne` continues to select the first highest-scoring candidate, while the parallel implementation resolves equal scores by their original index.

### 🟡 MEDIUM — Lock contention in parallel ExtractOne (`ResultExtractor.Parallel.cs:43-46`)

Every thread that finds a qualifying candidate acquires the lock. For large datasets with many candidates above cutoff, this serializes all merges. Consider merging locally first (already done), then doing one final merge at the end instead of locking per-thread.

---

## ✅ Good Patterns

- `BestCandidate<T>` struct with thread-local state for parallel merge is correct
- Direct selection avoids creating intermediate `ExtractedResult` instances for every candidate
- Cutoff filtering before heap insertion avoids unnecessary allocations
- `ScoreParallel` writes to distinct array indices — safe without synchronization

---

## Overall Assessment

The refactoring is sound and reduces per-candidate allocation and selection overhead without changing the established tie-breaking behavior.
