using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateBallDemo.Helpers
{
    public static class MissingLinq
    {
        /// <summary>
        /// Batch is an extension method that splits an IEnumerable's contents into a series of Lists.
        /// The lists will each be of length <paramref name="batchSize"/>, with the possible exception of the final List.
        /// If the <paramref name="source"/> does not end on a batch border, the final List will be a smaller batch containing the items left over after the last full-size batch
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="batchSize"/> is not greater than zero</exception>
        /// <typeparam name="T">The type of items in the source</typeparam>
        /// <param name="source">An IEnumerable to split into batches</param>
        /// <param name="batchSize">The desired length of each batch. Must be greater than zero</param>
        /// <returns>An IEnumerable of Lists of T</returns>
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive");
            }

            var list = new List<T>(batchSize);
            foreach (var item in source)
            {
                list.Add(item);
                if (list.Count == batchSize)
                {
                    yield return list;
                    list = new List<T>(batchSize);
                }
            }

            if (list.Any())
            {
                yield return list;
            }
        }

        /// <summary>
        /// Runs a function repeatedly and emits the result of each run as an IEnumerable
        /// </summary>
        /// <typeparam name="T">The type of items in the resulting IEnumerable</typeparam>
        /// <param name="func">The function to be run to produce each value</param>
        /// <returns>An IEnumerable of each successive result of <paramref name="func"/></returns>
        public static IEnumerable<T> RepeatCall<T>(Func<T> func)
        {
            while (true)
            {
                yield return func();
            }
        }


        /// <summary>
        /// Extension method to add side-effects to the materialisation of an IEnumerable.
        ///
        /// This method runs an Action on each item in an IEnumerable and then emits the original item to the resulting IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action) => source.Select(item =>
        {
            action(item);
            return item;
        });
    }
}
