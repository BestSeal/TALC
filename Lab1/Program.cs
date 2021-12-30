using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    class Program
    {
        static void Main()
        {
            string? expression = null;
            while (expression == null)
            {
                Console.WriteLine("Enter an expression:");
                expression = Console.ReadLine();
            }

            var parser = new TokenParser(expression.ToLower().Replace(" ", ""));

            var tokenStack = new Stack<Token>();
            var tokenQueue = new Queue<Token>();

            foreach (var token in parser.GetNextToken())
            {
                switch (token.TokenType)
                {
                    case TokenType.LeftBracket:
                        tokenStack.Push(token);
                        break;
                    case TokenType.RightBracket:
                        break;
                    case TokenType.Function:
                        tokenStack.Push(token);
                        break;
                    case TokenType.BinaryOperator:
                        break;
                    case TokenType.BinaryAndUnaryOperator:
                        break;
                    case TokenType.UnaryOperator:
                        break;
                    case TokenType.ArgumentSeparator:
                        while (tokenStack.TryPeek(out var peekedToken) &&
                               peekedToken.TokenType != TokenType.LeftBracket)
                        {
                            tokenQueue.Enqueue(tokenStack.Pop());
                        }

                        if (!tokenStack.Any() || tokenStack.Peek().TokenType != TokenType.LeftBracket)
                        {
                        }

                        break;
                    case TokenType.NumberSeparator:
                        break;
                    case TokenType.Number:
                        tokenQueue.Enqueue(token);
                        break;
                    case TokenType.Invalid:
                        throw new ArgumentException($"Invalid token found {token}");
                }
            }
        }
    }
}