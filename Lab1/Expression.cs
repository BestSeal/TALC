using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public class Expression
    {
        private string ExpressionString { get; }
        private Stack<Token> TokenStack { get; }
        private Queue<Token> TokenQueue { get; }
        private Stack<Token> EquationStack { get; }
        private TokenParser Parser { get; }

        public Expression(string expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            TokenStack = new Stack<Token>();
            TokenQueue = new Queue<Token>();
            EquationStack = new Stack<Token>();
            ExpressionString = expression.ToLower().Replace(" ", "");
            Parser = new TokenParser(ExpressionString);
        }

        public double Evaluate()
        {
            var tokens = Parser.GetNextToken().ToList();

            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].StringValue is "-" or "+")
                {
                    if (i == 0 || tokens[i - 1].TokenType is TokenType.LeftBracket)
                    {
                        tokens[i].TokenType = TokenType.UnaryOperator;
                    }
                }
            }

            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.LeftBracket:
                        TokenStack.Push(token);
                        break;
                    case TokenType.RightBracket:
                        Token peekedToken;
                        while (TokenStack.TryPeek(out peekedToken) && peekedToken.TokenType != TokenType.LeftBracket)
                        {
                            TokenQueue.Enqueue(TokenStack.Pop());
                        }
                        if (!TokenStack.TryPeek(out peekedToken) || peekedToken.TokenType != TokenType.LeftBracket)
                        {
                            throw new ArgumentException("Missing closing bracket.");
                        }
                        else if (peekedToken.TokenType == TokenType.LeftBracket)
                        {
                            TokenStack.Pop();
                        }
                        else if (peekedToken.TokenType == TokenType.Function)
                        {
                            TokenQueue.Enqueue(TokenStack.Pop());
                        }
                        break;
                    case TokenType.Function:
                        TokenStack.Push(token);
                        break;
                    case TokenType.BinaryOperator:
                    case TokenType.UnaryOperator:
                        while (TokenStack.TryPeek(out peekedToken) && peekedToken.TokenType is
                                   TokenType.BinaryOperator or TokenType.UnaryOperator or TokenType.Function &&
                               peekedToken.TokenPriority <= token.TokenPriority)
                        {
                            TokenQueue.Enqueue(TokenStack.Pop());
                        }
                        TokenStack.Push(token);
                        break;
                    case TokenType.ArgumentSeparator:
                        while (TokenStack.TryPeek(out peekedToken) && peekedToken.TokenType != TokenType.LeftBracket)
                        {
                            TokenQueue.Enqueue(TokenStack.Pop());
                        }

                        if (!TokenStack.TryPeek(out peekedToken) || peekedToken.TokenType != TokenType.LeftBracket)
                        {
                            throw new ArgumentException("Missing argument separator or opening bracket");
                        }
                        break;
                    case TokenType.Number:
                        TokenQueue.Enqueue(token);
                        break;
                    case TokenType.NumberSeparator:
                    case TokenType.Invalid:
                        throw new ArgumentException($"Found invalid token '{token.StringValue}'");
                }
            }

            while (TokenStack.Count > 0)
            {
                var token = TokenStack.Pop();
                if (token.TokenType == TokenType.LeftBracket)
                {
                    throw new ArgumentException("Missing bracket");
                }

                TokenQueue.Enqueue(token);
            }
        
            foreach (var token in TokenQueue)
            {
                if (token.TokenType == TokenType.Number)
                {
                    EquationStack.Push(token);
                }
                else
                {
                    switch (token.StringValue)
                    {
                        case "+":
                            if (token.TokenType == TokenType.UnaryOperator && TryEvaluateOperator(o => o)){}
                            else if (TryEvaluateOperator((o1, o2) => o1 + o2)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "-":
                            if (token.TokenType == TokenType.UnaryOperator && TryEvaluateOperator(o => -o)){}
                            else if (TryEvaluateOperator((o1, o2) => o1 - o2)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "*":
                            if (TryEvaluateOperator((o1, o2) => o1 * o2)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "/":
                            if (TryEvaluateOperator((o1, o2) => o1 / o2)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "^":
                            if (TryEvaluateOperator(Math.Pow)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "sin":
                            if (TryEvaluateFunction(Math.Sin)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "cos":
                            if (TryEvaluateFunction(Math.Cos)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "log":
                            if (TryEvaluateFunction((o, logBase) => Math.Log(o, logBase))){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                        case "pow":
                            if (TryEvaluateFunction(Math.Pow)){}
                            else
                            {
                                throw new ArgumentException($"Missing operand for {token.StringValue}");
                            }
                            break;
                    }
                }
            }
        
            if (EquationStack.TryPop(out var result) && result.DoubleValue.HasValue)
            {
                return result.DoubleValue.Value;
            }

            throw new Exception($"Unknown error with result: {result?.DoubleValue}");
        }
    
        private bool TryEvaluateFunction(Func<double, double> func) => TryEvaluateOperator(func);
    
        private bool TryEvaluateFunction(Func<double, double, double> func) => TryEvaluateOperator(func);
    
        private bool TryEvaluateOperator(Func<double, double, double> binaryOperator)
        {
            if (EquationStack.Count >= 2)
            {
                var secondOperand = EquationStack.Pop().DoubleValue;
                var firstOperand = EquationStack.Pop().DoubleValue;
                if (secondOperand.HasValue && firstOperand.HasValue)
                {
                    EquationStack.Push(new Token(TokenType.Number, -1, 
                        binaryOperator.Invoke(firstOperand.Value, secondOperand.Value)));

                    return true;
                }
            }

            return false;
        }
    
        private bool TryEvaluateOperator(Func<double, double> binaryOperator)
        {
            if (EquationStack.Count >= 1)
            {
                var firstOperand = EquationStack.Pop().DoubleValue;
                if (firstOperand.HasValue)
                {
                    EquationStack.Push(new Token(TokenType.Number, -1, 
                        binaryOperator.Invoke(firstOperand.Value)));   
                }

                return true;
            }

            return false;
        }
    }
}