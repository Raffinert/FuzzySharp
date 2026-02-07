using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

public static partial class Process
{
    public static partial class Parallel
    {
        #region ExtractAll

        /// <summary>
        /// Creates a list of ExtractedResult which contain all the choices with
        /// their corresponding score where higher is more similar
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<string>> ExtractAll(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            processor ??= DefaultStringProcessor;
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractWithoutOrder(query, choices, processor, scorer, cutoff, parallelOptions);
        }


        /// <summary>
        /// Creates a list of ExtractedResult which contain all the choices with
        /// their corresponding score where higher is more similar
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractWithoutOrder(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        /// <summary>
        /// Creates a list of ExtractedResult which contain all the choices with
        /// their corresponding score where higher is more similar
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            string query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractWithoutOrder(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        #endregion

        #region ExtractTop

        /// <summary>
        /// Creates a sorted list of ExtractedResult  which contain the
        /// top limit most similar choices
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="limit"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<string>> ExtractTop(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            IRatioScorer scorer = null,
            int limit = 5,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            processor ??= DefaultStringProcessor;
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractTop(query, choices, processor, scorer, limit, cutoff, parallelOptions);
        }

        /// <summary>
        /// Creates a sorted list of ExtractedResult  which contain the
        /// top limit most similar choices
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="limit"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer = null,
            int limit = 5,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractTop(query, choices, processor, scorer, limit, cutoff, parallelOptions);
        }

        #endregion

        #region ExtractSorted

        /// <summary>
        /// Creates a sorted list of ExtractedResult with the closest matches first
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<string>> ExtractSorted(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            processor ??= DefaultStringProcessor;
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractSorted(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        /// <summary>
        /// Creates a sorted list of ExtractedResult with the closest matches first
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractSorted(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        #endregion

        #region ExtractOne

        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static ExtractedResult<string> ExtractOne(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            processor ??= DefaultStringProcessor;
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractOne(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="choices"></param>
        /// <param name="processor"></param>
        /// <param name="scorer"></param>
        /// <param name="cutoff"></param>
        /// <param name="parallelOptions"></param>
        /// <returns></returns>
        public static ExtractedResult<T> ExtractOne<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer = null,
            int cutoff = 0,
            ParallelOptions parallelOptions = null)
        {
            scorer ??= DefaultScorer;
            return ResultExtractor.Parallel.ExtractOne(query, choices, processor, scorer, cutoff, parallelOptions);
        }

        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="choices"></param>
        /// <returns></returns>
        public static ExtractedResult<string> ExtractOne(string query, ParallelOptions parallelOptions = null, params string[] choices)
        {
            return ResultExtractor.Parallel.ExtractOne(query, choices, DefaultStringProcessor, DefaultScorer, parallelOptions: parallelOptions);
        }

        #endregion
    }
}