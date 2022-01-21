using System;
using System.IO;

public class Main
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Geese are cool!");
		SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer("src/main/resources/grammar.txt");
		try
		{
			syntaxAnalyzer.call("src/main/resources/cfile.txt");
		}
		catch (IOException e)
		{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}
	}
}
