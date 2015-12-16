using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using MathNet.Numerics;

namespace Expressions
{
    class Parser
    {
        // TODO:
        // * Functions
        // * Negative expressions
        // * Factorial of expressions

        // Note: Precedence of ParenFunc is set to lowest for simplicity in getRPN()
        enum Precedence { ParenFunc, AddSub, MultDiv, Pow, Fact };
        enum Operator { Add, Sub, Mult, Div, Pow, Fact, LeftParen, RightParen, Func };

        // <summary>
        // Parse and return the result of a mathematical expression given in infix notation.</summary>
        public static double GetResult(ReadOnlyCollection<string> symbols)
        {
            symbols = rewriteNegitives(symbols);
            ReadOnlyCollection<string> RPN = getRPN(symbols);
            return evaluateRPN(RPN);
        }

        // <summary>
        // Return operator precedence.</summary>
        private static Precedence getPrecedence(Operator oper)
        {
            if (oper == Operator.Add || oper == Operator.Sub)
                return Precedence.AddSub;
            else if (oper == Operator.Mult || oper == Operator.Div)
                return Precedence.MultDiv;
            else if (oper == Operator.Pow)
                return Precedence.Pow;
            else if (oper == Operator.Fact)
                return Precedence.Fact;
            else
                return Precedence.ParenFunc;
        }

        // <summary>
        // Return the type of the operator in string.</summary>
        private static Operator getOperatorType(string oper)
        {
            switch (oper)
            {
                case "+":
                    return Operator.Add;
               case "-":
                    return Operator.Sub;
                case "*":
                    return Operator.Mult;
                case "/":
                    return Operator.Div;
                case "^":
                    return Operator.Pow;
                case "!":
                    return Operator.Fact;
                case "(":
                    return Operator.LeftParen;
                case ")":
                    return Operator.RightParen;
                default:
                    return Operator.Func;
            }
        }

        // <summary>
        // Return true if string is numeric.</summary>
        private static bool isNumeric(string str)
        {
            double tmp;
            if (double.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out tmp))
                return true;
            else
                return false;
        }

        // <summary>
        // Convert numeric symbol to double.</summary>
        // <remarks>No error checking. Test symbol first with isNumeric().
        private static double parseNumeric(string symbol)
        {
            return double.Parse(symbol, new CultureInfo("en-US"));
        }

        // <summary>
        // Search symbols collection for minus sign/numeral combinations that should be interpreted
        // as negative numbers and rewrite them as one symbol.</summary>
        private static ReadOnlyCollection<string> rewriteNegitives(ReadOnlyCollection<string> symbols)
        {
            if (symbols.Count < 2)
                return symbols;

            List<string> symbolsList = symbols.ToList<string>();

            for (int i = 0; i < symbolsList.Count - 1; i++)
            {
                // Examine three symbols in a row
                string symBefore =  i > 0 ? symbolsList.ElementAt(i - 1) : "";
                string symAt = symbolsList.ElementAt(i);
                string symAfter = symbolsList.ElementAt(i + 1);

                if (!isNumeric(symBefore)
                    && symBefore != ")"
                    && symBefore != "!"
                    && symAt == "-"
                    && isNumeric(symAfter))
                {
                    symbolsList.RemoveRange(i, 2);
                    symbolsList.Insert(i, symAt + symAfter);
                }
            }

            return symbolsList.AsReadOnly();
        }

        // <summary>
        // Reorder a mathematical symbol list written in infix notation to
        // reverse polish notation and return the result.</summary>
        private static ReadOnlyCollection<string> getRPN(ReadOnlyCollection<string> symbols)
        {
            List<string> RPN = new List<string>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string symbol in symbols)
            {
                if (isNumeric(symbol))
                {
                    // Found numeric
                    RPN.Add(symbol);
                }
                else
                {
                    // Found operator
                    Operator operatorType = getOperatorType(symbol);
                    Precedence operatorPrecedence = getPrecedence(operatorType);

                    if (operatorType == Operator.Fact) {
                        // Add factorial operator directly to out que
                        RPN.Add(symbol);
                        continue;
                    }

                    if (operatorType == Operator.RightParen)
                    {
                        // Pop until we find a left parenthesis (and maybe a function)
                        while (true)
                        {
                            if (operatorStack.Count == 0)
                                throw new ArgumentException("Parser: Unmatched right parenthesis");
                            string oper = operatorStack.Pop();
                            operatorType = getOperatorType(oper);
                            if (operatorType == Operator.LeftParen)
                                break;
                            RPN.Add(oper);
                        }

                        if (operatorStack.Count > 0 && getOperatorType(operatorStack.Peek()) == Operator.Func)
                            RPN.Add(operatorStack.Pop());

                        continue;
                    }

                    if (operatorStack.Count > 0
                        && operatorType != Operator.LeftParen
                        && operatorType != Operator.Func)
                    {
                        // As long as the operator on top of the stack has higher precedence: pop.
                        while (operatorStack.Count > 0
                            && operatorPrecedence <= getPrecedence(getOperatorType(operatorStack.Peek())))
                        {
                            RPN.Add(operatorStack.Pop());
                        }
                    }

                    // Push operator onto stack
                    operatorStack.Push(symbol);
                }
            }

            // End of input: Pop operators remaining on stack
            while (operatorStack.Count > 0)
                RPN.Add(operatorStack.Pop());

            return RPN.AsReadOnly();
        }

        // <summary>
        // Return the calculated result of a mathematical expression given in RPN notation.</summary>
        private static double evaluateRPN(ReadOnlyCollection<string> RPN)
        {
            Stack<double> numStack = new Stack<double>();

            foreach (string symbol in RPN)
            {
                if (isNumeric(symbol))
                    numStack.Push(parseNumeric(symbol));
                else
                {
                    // Operator found
                    Operator oper = getOperatorType(symbol);
                    double operandL, operandR;

                    if (oper == Operator.Fact)
                    {
                        // Factorial
                        if (numStack.Count < 1)
                            throw new ArgumentException("Parser: Malformed expression.");
                        double operand = numStack.Pop();
                        if (operand == 0)
                            numStack.Push(1);
                        else if (operand % 1 == 0 && operand > 0 && operand <= 170)
                            numStack.Push(SpecialFunctions.Factorial((int)operand));
                        else if (operand % 1 != 0 && operand >= -170 && operand <= 170)
                            numStack.Push(SpecialFunctions.Gamma(operand + 1D));
                        else
                            throw new ArgumentException("Parser: Factorial operand out of range.");

                    }
                    else
                    {
                        if (numStack.Count < 2)
                            throw new ArgumentException("Parser: Malformed expression.");
                        operandR = numStack.Pop();
                        operandL = numStack.Pop();
                        switch (oper)
                        {
                            case Operator.Add:
                                numStack.Push(operandL + operandR);
                                break;
                            case Operator.Sub:
                                numStack.Push(operandL - operandR);
                                break;
                            case Operator.Mult:
                                numStack.Push(operandL * operandR);
                                break;
                            case Operator.Div:
                                numStack.Push(operandL / operandR);
                                break;
                            case Operator.Pow:
                                numStack.Push(Math.Pow(operandL, operandR));
                                break;
                        }
                    }
                }

            }

            if (numStack.Count != 1)
                throw new ArgumentException("Parser: Malformed expression.");

            return numStack.Pop();
        }
    }
}
