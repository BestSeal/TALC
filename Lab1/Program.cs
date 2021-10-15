using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    class Program
    {
        static void Main()
        {
            string expression = null;
            while (expression == null)
            {
                Console.WriteLine("Enter an expression:");
                expression = Console.ReadLine();
            }

            var parser = new TokenParser(expression.ToLower().Replace(" ", ""));

            var tokens = new List<Token>();
            
            foreach (var token in parser.GetNextToken())
            {
                tokens.Add(token);
            }

            Console.WriteLine(string.Join("", tokens.Select(x => x.StringValue).ToArray()));
        }
    }
}