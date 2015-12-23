using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Expressions
{
    // <summary>
    // Tools to extract mathematical symbols and numerics from a mathematical infix expression.
    // Public methods: static ReadOnlyCollection<string> GetSymbols(string expression)
    // </summary>
    class Lexer
    {
        // Mathematical operators
        private static string _operators = "+ | - | * | / | ( | ) | ^ | !";
        private static string _functions = Function.KeywordsAsVBarSeparatedString;

        // <summary>
        // Return symbols in an expression as a collection. Symbols are keywords and
        // operators defined above, and decimal numbers.</summary>
        // <param name="expression">The mathematical expression to be evaluated.</param>
        // <returns>Returns a collection of symbols as strings.</returns>
        public static ReadOnlyCollection<string> GetSymbols(string expression)
        {
            List<string> symbols = new List<string>();
            string curSym;

            // Regex pattern for numbers
            string numPattern = @"(^(\d*\.)?\d+)";

            // Regex pattern for operators and functions (see member variables above)
            string opPattern = preparePattern(_operators);
            string fnPattern = preparePattern(_functions);

            // Concatenate patterns
            string pattern = numPattern + opPattern + fnPattern;

            // Remove all whitespace from expression and convert to lower case
            expression = removeWhitespace(expression).ToLower();

            // Extract symbols and add them to list
            while (expression.Length > 0)
            {
                curSym = Regex.Match(expression, pattern).ToString();

                if (curSym.Length > 0)
                    symbols.Add(curSym);
                else
                    throw new ArgumentException("Lexer: Found unknown symbols.");

                expression = expression.Substring(curSym.Length);
            }

            return symbols.AsReadOnly();
        }

        // <summary>
        // Return a string like str but with all whitespace removed.</summary>
        private static string removeWhitespace(string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }

        // <summary>
        // Prepare pattern for concatenation in GetSymbols(). str contains operators and/or
        // keywords separated by vertical bars. Return string have special chars in regex   
        // escaped except for vertical bars. Also, ^'s are added before each keyword.</summary>
        private static string preparePattern(string str)
        {
            string pattern = "|" + removeWhitespace(str); // Remove whitespace
            pattern = Regex.Escape(pattern); // Escape special chars
            pattern = Regex.Replace(pattern, @"\\\|", "|^"); // Restore |'s and insert ^'s

            return pattern;
        }
    }
}
