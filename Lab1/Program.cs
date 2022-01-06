using System;

namespace Lab1;

class Program
{
    static void Main()
    {
        // -1 + (3 * 2 + 4 / 2) + pow(2,2) - 3.2

        string? expressionString = null;
        while (expressionString == null)
        {
            Console.WriteLine("Enter an expression:");
            expressionString = Console.ReadLine();
        }
        try
        {
            var expression = new Expression(expressionString);
            var result = expression.Evaluate();
            
            Console.WriteLine($"\nResult: {result}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Found error while executing expression {e}");
        }
    }
}