using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Expressions
{
    // <summary>
    // Implementations and methods for evaluating mathematical functions.</summary>
    static class Function
    {
        // <summary>
        // Describes a functions name and the number of arguments it takes.</summary>
        private struct Keyword
        {
            public Keyword(string name, int numberOfArguments, Func<double[], double> function)
            {
                this.name = name;
                this.numberOfArguments = numberOfArguments;
                this.function = function;
            }
            public string name;
            public int numberOfArguments;
            public Func<double[], double> function;
        }

        private static Keyword[] keywords = {
                                                new Keyword("sin", 1, new Func<double[],double>(sin)),
                                                new Keyword("cos", 1, new Func<double[], double>(cos)),
                                                new Keyword("tan", 1, new Func<double[],double>(tan)),
                                                new Keyword("abs", 1, new Func<double[],double>(abs)),
                                                new Keyword("sqrt", 1, new Func<double[],double>(sqrt))
                                            };

        private static string keywordsAsVBarSeparatedString = constructVBarSeparatedStringOfKeywords();

        // <summary>
        // Get valid keywords as a vertical bar separated string.</summary>
        public static string KeywordsAsVBarSeparatedString
        {
            get { return keywordsAsVBarSeparatedString; }
        }

        // <summary>
        // Get the number of arguments for function specified by keyword.</summary>
        public static int NumberOfArguments(string keyword)
        {
            var kw = from Keyword in keywords where Keyword.name == keyword select Keyword.numberOfArguments;
            if (kw.Count() == 0)
                throw new ArgumentException("Function: NumberOfArguments: No such keyword.");

            return kw.ElementAt(0);
        }

        // <summary>
        // Evaluate function for given keyword and return the result.</summary>
        public static double Evaluate(string keyword, double[] arguments)
        {
            var fn = (from Keyword in keywords where Keyword.name == keyword select Keyword.function).ElementAt(0);
            double result = fn(arguments);
            return result;
        }

        // <summary>
        // Return a string with keywords separated by vertical bars.</summary>
        private static string constructVBarSeparatedStringOfKeywords()
        {
            string kws = "";
            foreach (Keyword kw in keywords)
            {
                kws += kw.name + "|";
            }

            return kws.Substring(0, kws.Length - 1);
        }

        // ***** FUNCTIONS *****

        private static double sin(double[] rad)
        {
            return Trig.Sin(rad[0]);
        }

        private static double cos(double[] rad)
        {
            return Trig.Cos(rad[0]);
        }

        private static double tan(double[] rad)
        {
            return Trig.Tan(rad[0]);
        }

        private static double abs(double[] x)
        {
            return x[0] < 0 ? 0 - x[0] : x[0];
        }

        private static double sqrt(double[] x)
        {
            return Math.Pow(x[0], 1D / 2D);
        }
    }
}
