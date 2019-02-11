using NUnit.Framework;
using System;
using System.Diagnostics.Contracts;

namespace Exercises.Chapter2
{
   // 1. Write a console app that calculates a user's Body-Mass Index:
   //   - prompt the user for her height in metres and weight in kg
   //   - calculate the BMI as weight/height^2
   //   - output a message: underweight(bmi<18.5), overweight(bmi>=25) or healthy weight
   // 2. Structure your code so that structure it so that pure and impure parts are separate
   // 3. Unit test the pure parts
   // 4. Unit test the impure parts using the HOF-based approach

   public static class Bmi
   {
        public static void Main()
        {
            Run(ReadDouble, Write);
        }

        internal static double ReadDouble(string message)
        {
            Console.WriteLine(message);
            return (double.TryParse(Console.ReadLine(), out var number)) ? number : 0d;
        }

        internal static void Write(string message)
        {
            Console.WriteLine(message);
        }

        [Pure]
        internal static void Run(Func<string, double> readDouble, Action<string> write)
        {
            var height = readDouble("enter height in metres.");
            var weight = readDouble("enter weight in kg.");
            var bmi = CalculateBmi(height, weight);
            var message = GetMessageFor(bmi);

            write(message);
        }

        [Pure]
        internal static double CalculateBmi(double weight, double height)
        {
            return weight / Math.Pow(height, 2);
        }

        [Pure]
        internal static string GetMessageFor(double bmi)
        {
            return
                (bmi < 18.5) ? "underweight" :
                (bmi > 25.0) ? "overweight" :
                "healthy weight";
        }
   }

    public class BmiTest
    {
        [TestCase(18.4, ExpectedResult = "underweight")]
        [TestCase(18.5, ExpectedResult = "healthy weight")]
        [TestCase(25.0, ExpectedResult = "healthy weight")]
        [TestCase(25.1, ExpectedResult = "overweight")]
        public string BmiMessageTest(double bmi)
        {
            return Bmi.GetMessageFor(bmi);
        }
    }
}
