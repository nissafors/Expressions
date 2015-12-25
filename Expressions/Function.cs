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
                                                new Keyword("sinh", 1, new Func<double[],double>(sinh)),
                                                new Keyword("sin", 1, new Func<double[],double>(sin)),
                                                new Keyword("asin", 1, new Func<double[],double>(asin)),
                                                new Keyword("cosh", 1, new Func<double[],double>(cosh)),
                                                new Keyword("cos", 1, new Func<double[], double>(cos)),
                                                new Keyword("acos", 1, new Func<double[],double>(acos)),
                                                new Keyword("tanh", 1, new Func<double[],double>(tanh)),
                                                new Keyword("tan", 1, new Func<double[],double>(tan)),
                                                new Keyword("atan", 1, new Func<double[],double>(atan)),
                                                new Keyword("degtorad", 1, new Func<double[],double>(degtorad)),
                                                new Keyword("radtodeg", 1, new Func<double[],double>(radtodeg)),
                                                new Keyword("abs", 1, new Func<double[],double>(abs)),
                                                new Keyword("sqrt", 1, new Func<double[],double>(sqrt)),
                                                new Keyword("max", 2, new Func<double[], double>(max)),
                                                new Keyword("min", 2, new Func<double[],double>(min)),
                                                new Keyword("floor", 1, new Func<double[],double>(floor)),
                                                new Keyword("ceiling", 1, new Func<double[],double>(ceiling)),
                                                new Keyword("exp", 1, new Func<double[],double>(exp)),
                                                new Keyword("ln", 1, new Func<double[],double>(ln)),
                                                new Keyword("log", 1, new Func<double[],double>(log)),
                                                new Keyword("mod", 2, new Func<double[],double>(mod))
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
            return Math.Sin(rad[0]);
        }

        private static double asin(double[] sin)
        {
            return Math.Asin(sin[0]);
        }

        private static double sinh(double[] rad)
        {
            return Math.Sinh(rad[0]);
        }

        private static double cos(double[] rad)
        {
            return Math.Cos(rad[0]);
        }

        private static double acos(double[] cos)
        {
            return Math.Acos(cos[0]);
        }

        private static double cosh(double[] rad)
        {
            return Math.Cosh(rad[0]);
        }

        private static double tan(double[] rad)
        {
            return Math.Tan(rad[0]);
        }

        private static double atan(double[] tan)
        {
            return Math.Atan(tan[0]);
        }

        private static double tanh(double[] rad)
        {
            return Math.Tanh(rad[0]);
        }

        private static double degtorad(double[] deg)
        {
            return (Math.PI / 180D) * deg[0];
        }

        private static double radtodeg(double[] rad)
        {
            return (180D / Math.PI) * rad[0];
        }

        private static double abs(double[] x)
        {
            return x[0] < 0 ? 0 - x[0] : x[0];
        }

        private static double sqrt(double[] x)
        {
            return Math.Sqrt(x[0]);
        }

        private static double max(double[] xy)
        {
            return xy[0] > xy[1] ? xy[0] : xy[1];
        }

        private static double min(double[] xy)
        {
            return xy[0] < xy[1] ? xy[0] : xy[1];
        }

        private static double floor(double[] x)
        {
            return Math.Floor(x[0]);
        }

        private static double ceiling(double[] x)
        {
            return Math.Ceiling(x[0]);
        }

        private static double round(double[] x)
        {
            return Math.Round(x[0]);
        }

        private static double exp(double[] x)
        {
            return Math.Exp(x[0]);
        }

        private static double ln(double[] x)
        {
            return Math.Log(x[0]);
        }

        private static double log(double[] x)
        {
            return Math.Log10(x[0]);
        }

        private static double mod(double[] xy)
        {
            return xy[0] % xy[1];
        }
    }
}
