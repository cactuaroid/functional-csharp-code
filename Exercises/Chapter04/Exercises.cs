using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;

namespace Exercises.Chapter4
{
    using static LaYumba.Functional.F;

    static class Exercises
    {
        // 1 Implement Map for ISet<T> and IDictionary<K, T>. (Tip: start by writing down
        // the signature in arrow notation.)

        // Map: ISet<T> -> (T -> R) -> ISet<R>
        public static ISet<R> Map<T, R>(this ISet<T> set, Func<T, R> mapper)
            => new HashSet<R>(set.Select(mapper));

        // Map: IDictionary<K, T> -> (T -> R) -> IDictionary<K, R>
        public static IDictionary<K, R> Map<K, T, R>(this IDictionary<K, T> dictionary, Func<T, R> mapper)
            => new Dictionary<K, R>(dictionary.Select((x) => new KeyValuePair<K, R>(x.Key, mapper(x.Value))));

        // 2 Implement Map for Option and IEnumerable in terms of Bind and Return.
        // Map: Option<T> -> (T -> R) -> Option<R>
        public static Option<R> Map<T, R>(this Option<T> option, Func<T, R> mapper)
            => option.Bind((x) => Some(mapper(x)));

        // Map: IEnumerable<T> -> (T -> R) -> IEnumerable<R>
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> sequence, Func<T, R> mapper)
            => sequence.Bind((x) => List(mapper(x)));

        // 3 Use Bind and an Option-returning Lookup function (such as the one we defined
        // in chapter 3) to implement GetWorkPermit, shown below. 

        // Then enrich the implementation so that `GetWorkPermit`
        // returns `None` if the work permit has expired.

        static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> people, string employeeId)
            => people.Lookup(employeeId)
                .Bind((x) => x.WorkPermit);

        static Option<WorkPermit> GetWorkPermit2(Dictionary<string, Employee> people, string employeeId)
            => people.Lookup(employeeId)
                .Bind((x) => x.WorkPermit)
                .Where((x) => IsUnexpiredWorkPermit(x, DateTime.Now));

        private static bool IsUnexpiredWorkPermit(WorkPermit workPermit, DateTime now)
            => workPermit.Expiry >= now;

        // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
        // employees who have left should be included).

        static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
            => employees
                .Bind(GetWorkingDuration)
                .Average((x) => x.TotalDays / 365);

        private static Option<TimeSpan> GetWorkingDuration(Employee employee)
            => employee.LeftOn.Map((leftOn) => leftOn - employee.JoinedOn);
    }

    public struct WorkPermit
    {
        public string Number { get; set; }
        public DateTime Expiry { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }
        public Option<WorkPermit> WorkPermit { get; set; }

        public DateTime JoinedOn { get; }
        public Option<DateTime> LeftOn { get; }
    }
}