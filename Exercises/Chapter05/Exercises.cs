using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Examples.Chapter3;

namespace Exercises.Chapter5
{
    public static class Exercises
    {
        // 1. Without looking at any code or documentation (or intllisense), write the function signatures of
        // `OrderByDescending`, `Take` and `Average`, which we used to implement `AverageEarningsOfRichestQuartile`:
        static decimal AverageEarningsOfRichestQuartile(List<Person> population)
           => population
              .OrderByDescending(p => p.Earnings)
              .Take(population.Count / 4)
              .Select(p => p.Earnings)
              .Average();

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, Func<T, IComparable> func)
        {
            var list = new List<T>(enumerable);
            var result = new T[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var max = list[i];

                for (int j = i; j < list.Count; j++)
                {
                    max = (func(list[j]).CompareTo(max) > 0) ? list[j] : max;
                }

                result[i] = max;
            }

            return result;
        }

        public static IEnumerable<T> Take<T>(this IEnumerable<T> enumerable, int count)
        {
            var enumerator = enumerable.GetEnumerator();

            for (int i = 0; i < count; i++)
            {
                enumerator.MoveNext();
                yield return enumerator.Current;
            }
        }

        public static decimal Average(this IEnumerable<decimal> enumerable)
        {
            var count = enumerable.Count();
            if (count == 0) { return default(decimal); }

            var enumerator = enumerable.GetEnumerator();

            decimal result = default(decimal);
            while (enumerator.MoveNext())
            {
                result = result + enumerator.Current / count;
            }

            return result;
        }

        // 2 Check your answer with the MSDN documentation: https://docs.microsoft.com/
        // en-us/dotnet/api/system.linq.enumerable. How is Average different?

        // 3 Implement a general purpose Compose function that takes two unary functions
        // and returns the composition of the two.

        public static Func<T, R2> Compose<T, R1, R2>(Func<R1, R2> f, Func<T, R1> g)
            => x => f(g(x));
    }
}
