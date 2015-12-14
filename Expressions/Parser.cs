using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Expressions
{
    class Parser
    {
        // TODO:
        // * Negative numbers
        // * Factorial
        // * Functions

        // Note: Precedence of ParenFunc is set to lowest for simplicity in getRPN()
        enum Precedence { ParenFunc, AddSub, MultDiv, Pow, Fact };
        enum Operator { Add, Sub, Mult, Div, Pow, Fact, LeftParen, RightParen, Func };

        // Parse and return the result of a mathematical expression given in infix notation.
        public static decimal GetResult(ReadOnlyCollection<string> symbols)
        {
            ReadOnlyCollection<string> RPN = getRPN(symbols);
            return evaluateRPN(RPN);
        }

        // Return the calculated result of a mathematical expression given in RPN notation.
        private static decimal evaluateRPN(ReadOnlyCollection<string> RPN)
        {
            Stack<decimal> numStack = new Stack<decimal>();

            foreach (string symbol in RPN)
            {
                if (isNumeric(symbol))
                    numStack.Push(decimal.Parse(symbol, new CultureInfo("en-US")));
                else
                {
                    // Operator found
                    Operator oper = getOperatorType(symbol);
                    decimal operandL, operandR;

                    // Is it not a binary operator?
                    if (oper == Operator.Fact)
                    {
                        if (numStack.Count < 1)
                            throw new ArgumentException("Parser: Malformed expression.");
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
                                numStack.Push((decimal)Math.Pow((double)operandL, (double)operandR));
                                break;
                        }
                    }
                }

            }

            if (numStack.Count != 1)
                throw new ArgumentException("Parser: Malformed expression.");

            return numStack.Pop();
        }

        // Return operator precedence
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

        // Return the type of the operator in string
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

        // Return true if string is numeric
        private static bool isNumeric(string str)
        {
            decimal tmp;
            if (decimal.TryParse(str, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out tmp))
                return true;
            else
                return false;
        }

        // Reorder a mathematical symbol list written in infix notation to
        // reverse polish notation and return the result.
        // Note: This implementation keeps parenthesises. Use removeParenRPN()
        // to remove them.
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
    }
}
