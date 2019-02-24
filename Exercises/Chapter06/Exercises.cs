using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;

namespace Exercises.Chapter6
{
    static class Exercises
    {
        // 1. Write a `ToOption` extension method to convert an `Either` into an
        // `Option`. Then write a `ToEither` method to convert an `Option` into an
        // `Either`, with a suitable parameter that can be invoked to obtain the
        // appropriate `Left` value, if the `Option` is `None`. (Tip: start by writing
        // the function signatures in arrow notation)

        // ToOption: Either<L, R> -> Option<R>
        public static Option<R> ToOption<L, R>(this Either<L, R> either)
            => either.Match(
                Left: _ => None,
                Right: right => Some(right));

        // ToEither: (Option<T>, (() -> L)) -> Either<L, T>
        public static Either<L, T> ToEither<L, T>(this Option<T> option, Func<L> left)
            => option.Match<Either<L, T>>(
                None: () => left(),
                Some: (right) => right);

        // 2. Take a workflow where 2 or more functions that return an `Option`
        // are chained using `Bind`.

        public static bool RegisterMessage(string message)
            => ValidateMessageLength(message)
                .Bind(ValidateMessageEndsInPeriod)
                .Match(
                    None: () => false,
                    Some: (x) =>
                    {
                        Register(x);
                        return true;
                    });

        private static Either<string, string> ValidateMessageLength(string message)
        {
            if (message.Length <= 100)
            {
                return Left("message must be <= 100 characters.");
            }
            else
            {
                return Right(message);
            }
        }

        private static Option<string> ValidateMessageEndsInPeriod(string message)
            => (message[message.Length - 1] == '.') ? None : Some(message);

        private static void Register(string message) { } // dummy

        // Then change the first one of the functions to return an `Either`.

        // This should cause compilation to fail. Since `Either` can be
        // converted into an `Option` as we have done in the previous exercise,
        // write extension overloads for `Bind`, so that
        // functions returning `Either` and `Option` can be chained with `Bind`,
        // yielding an `Option`.

        // Bind: (Either<L, R>, Func<R, Option<RR>>) -> Option<RR>
        public static Option<RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Option<RR>> func)
            => either.ToOption().Bind(func);

        // Bind: (Option<R>, Func<R, Either<L, RR>) -> Option<RR>
        public static Option<RR> Bind<L, R, RR>(this Option<R> option, Func<R, Either<L, RR>> func)
            => option.Match(
                None: () => None,
                Some: (x) => func(x).ToOption());

        // 3. Write a function `Safely` of type ((() → R), (Exception → L)) → Either<L, R> that will
        // run the given function in a `try/catch`, returning an appropriately
        // populated `Either`.

        public static Either<L, R> Safely<L, R>(Func<R> rightFunc, Func<Exception, L> leftFunc)
        {
            try
            {
                return rightFunc.Invoke();
            }
            catch (Exception ex)
            {
                return leftFunc(ex);
            }
        }

        // 4. Write a function `Try` of type (() → T) → Exceptional<T> that will
        // run the given function in a `try/catch`, returning an appropriately
        // populated `Exceptional`.

        public static Exceptional<T> Try<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
