using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Helper
{
    public static class Helper
    {
        /// <summary>
        /// Expand parts of exception and its inner exception to a single message.
        /// </summary>
        /// <param name="ex">the exception to be expanded</param>
        /// <param name="additionalInfo">optional additional comment\info supplied by caller</param>
        /// <returns>the concatenated Message and StackTrace parts</returns>
        public static String Expand(this Exception ex, String additionalInfo = "")
        {
            var value = new StringBuilder(" Message: " + ex.Message + Environment.NewLine);
            if (!String.IsNullOrEmpty(additionalInfo))
                value.AppendLine("ExtraInfo: " + additionalInfo + Environment.NewLine);
            value.AppendLine("StackTrace(outer): " + ex.StackTrace);
            if (ex.InnerException != null)
            {
                value.AppendLine("InnerExceptionMessage: " + ex.InnerException.Message);
                value.AppendLine("InnerExceptionStackTrace: " + ex.InnerException.StackTrace);
            }
            return value.ToString();
        }

        //Extra methods from Michael de Vera:
        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> sequence
                                            , Func<TSource, TResult> projector // operate on each TSource member of the sequence producing and object of type TResult.
                                            , Boolean skipNullProjections = false)
        {
            var result = new List<TResult>();
            if (!skipNullProjections || typeof(TResult).IsValueType) // fast or slow paths.
                foreach (var record in sequence)
                    result.Add(projector(record));
            else
                foreach (var record in sequence)
                {
                    var projection = projector(record);
                    if (null != projection)
                        result.Add(projection);
                }
            return result;
        }


        /// <summary>
        /// Side-effecting by traversal with action against specified sequence.
        /// </summary>
        /// <typeparam name="TSource">Domain of action method.</typeparam>
        /// <param name="sequence">enumerable of TSource objects</param>
        /// <param name="action">Method applied to each item</param>    
        public static void Repeat<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence == null) return;
            foreach (T item in sequence)
                action(item);
        }

        /// <summary>
        /// Side-effecting by traversal with action against specified sequence.
        /// </summary>
        /// <typeparam name="TSource">Domain of action method.</typeparam>
        /// <param name="sequence">IQueryable of TSource objects</param>
        /// <param name="action">Method applied to each item</param>    
        public static void Repeat<T>(this IQueryable<T> sequence, Action<T> action)
        {
            if (sequence == null) return;
            foreach (T item in sequence)
                action(item);
        }

        public static List<TResult> ToList<TSource, TResult>(this IQueryable<TSource> query
                                           , Func<TSource, TResult> projector
                                           , Boolean skipNullProjections = false)
        {
            var result = new List<TResult>();
            if (!skipNullProjections || typeof(TResult).IsValueType)
                foreach (var record in query)
                    result.Add(projector(record));
            else
                foreach (var record in query)
                {
                    var projection = projector(record);
                    if (null != projection)
                        result.Add(projection);
                }
            return result;
        }

    } //class
}
