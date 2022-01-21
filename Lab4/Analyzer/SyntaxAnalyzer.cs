using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SyntaxAnalyzer
{

	internal enum SymType
	{
		TER,
		NON_TER,
		SPEC
	}

	internal Symbol start_symbol;
	internal IDictionary<Symbol, ISet<Production>> grammars;
	internal ISet<Symbol> non_terminal_alphabet;
	internal ISet<Symbol> terminal_alphabet;

	internal IDictionary<Symbol, ISet<Symbol>> firsts;
	internal IDictionary<Symbol, ISet<Symbol>> follows;

	internal IDictionary<Symbol, IDictionary<Symbol, Production>> symbol_matrix; //symbol_matrix.get(non_terminal).get(terminal) -> production
	internal IDictionary<Symbol, ISet<Symbol>> synchro_map;

	public SyntaxAnalyzer(string filename)
	{
		grammars = new HashMapHelper<Symbol, ISet<Production>>();
		non_terminal_alphabet = new HashSet<Symbol>();
		terminal_alphabet = new HashSet<Symbol>();

		firsts = new Dictionary<Symbol, ISet<Symbol>>();
		follows = new Dictionary<Symbol, ISet<Symbol>>();

		try
		{
			using (StreamReader br = new StreamReader(filename))
			{
				for (string line; (line = br.ReadLine()) != null;)
				{
					string bufferSymbol = "";
					SymType symType = null;
					Symbol grammarKey = null;
					ISet<Production> grammarValue = new HashSet<Production>();
					Production production = new Production();
					for (int i = 0; i < line.length(); i++)
					{
						switch (line.charAt(i))
						{
						case '<':
						{
								if (symType == null || symType.Equals(SymType.SPEC))
								{
									symType = SymType.NON_TER;
								}
								else if (symType.Equals(SymType.TER))
								{
									bufferSymbol += "<";
								}

								break;
						}
						case '>':
						{
								if (symType != null && symType.Equals(SymType.NON_TER))
								{
									Symbol symbol = new Symbol(false, bufferSymbol);
									non_terminal_alphabet.Add(symbol);
									if (start_symbol == null)
									{
										start_symbol = symbol;
									}
									if (grammarKey == null)
									{
										grammarKey = symbol;
									}
									else
									{
										production.addSymbolToProduction(symbol);
									}
									symType = null;
									bufferSymbol = "";
								}
								else if (symType != null && symType.Equals(SymType.TER))
								{
									bufferSymbol += ">";
								}

								break;
						}
						case ':':
						{
								if (symType == null)
								{
									symType = SymType.SPEC;
								}
								else if (symType.Equals(SymType.TER))
								{
									bufferSymbol += ":";
								}

								break;
						}
						case '\'':
						{
								if (symType == null || symType.Equals(SymType.SPEC))
								{
									symType = SymType.TER;
    
								}
								else if (symType.Equals(SymType.TER))
								{
									Symbol symbol = new Symbol(true, bufferSymbol);
									terminal_alphabet.Add(symbol);
									production.addSymbolToProduction(symbol);
									bufferSymbol = "";
									symType = null;
								}

								break;
						}
						case ' ':
						{
								Symbol symbol = new Symbol(true, " ");
								terminal_alphabet.Add(symbol);
								production.addSymbolToProduction(symbol);
								bufferSymbol = "";
								symType = null;

								break;
						}
						case '|':
						{
								if (symType != null && symType.Equals(SymType.SPEC))
								{
									Symbol symbol = new Symbol(true, "");
									production.addSymbolToProduction(symbol);
									bufferSymbol = "";
									symType = null;
								}
								grammarValue.Add(production);
								production = new Production();

								break;
						}
						default:
							bufferSymbol += line.charAt(i);
							break;
						}
					}
					grammarValue.Add(production);
					grammars[grammarKey] = grammarValue;
				}
			}
		}
		catch (IOException e)
		{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}

		foreach (var entry in grammars.SetOfKeyValuePairs())
		{
			var non_terminal = entry.Key;
			ISet<Symbol> first_set = getFirst(non_terminal);
			firsts[non_terminal] = first_set;
		}
		fillFollow();
		fillMatrix();
		fill_synchro();
	}

	public virtual string grammarsToString()
	{
		StringBuilder retVal = new StringBuilder();
		foreach (var entry in grammars.SetOfKeyValuePairs())
		{
			retVal.Append(entry.Key.ToString());
			retVal.Append(":");
			foreach (var production in entry.Value)
			{
				retVal.Append(production.ToString());
				retVal.Append("|");
			}
			retVal.Remove(retVal.Length - 1, 1);
			retVal.Append("\n");
		}
		return retVal.ToString();
	}


	private ISet<Symbol> getFirst(Symbol symbol)
	{
		Symbol eps = new Symbol(true, "");
		bool hasEps = false;
		bool hasIncluded = false;
		ISet<Symbol> returnSet = new HashSet<Symbol>();
		if (symbol.isTerminal)
		{
			returnSet.Add(symbol);
			return returnSet;
		}
		foreach (Production pr in grammars[symbol])
		{
			Symbol firstSymbol = pr.first();
			if (firstSymbol.isTerminal)
			{
				returnSet.Add(firstSymbol);
				if (firstSymbol.Equals(eps))
				{
					hasEps = false;
				}
			}
			else
			{
				ISet<Symbol> first_from_first_symbol = getFirst(firstSymbol);
				int before_remove_size = first_from_first_symbol.Count;
				first_from_first_symbol.removeIf(iterable => iterable.Equals(eps));
				if (before_remove_size == first_from_first_symbol.Count)
				{
					hasEps = false;
				}
				else
				{
					if (!hasIncluded)
					{
						hasEps = true;
					}
				}
				hasIncluded = true;
				returnSet.addAll(first_from_first_symbol);
			}
		}
		if (hasEps)
		{
			returnSet.Add(eps);
		}
		return returnSet;
	}

	private void fillFollow()
	{
		Symbol eps = new Symbol(true, "");
		foreach (var entry in grammars.SetOfKeyValuePairs())
		{
			ISet<Production> production_set = entry.Value;
			foreach (Production prod in production_set)
			{
				for (int i = 0; i < prod.symbolList.Count - 1; i++)
				{
					Symbol current_symbol = prod.symbolList[i];
//                    if (!current_symbol.isTerminal) {
					Symbol next_symbol = prod.symbolList[i + 1];
					if (!follows.ContainsKey(current_symbol))
					{
						follows[current_symbol] = new HashSet<Symbol>();
						if (current_symbol.Equals(start_symbol))
						{
							follows[current_symbol].Add(eps);
						}
					}
					ISet<Symbol> next_symbol_firsts;
					next_symbol_firsts = new HashSet<Symbol>();
					if (next_symbol.Terminal && !next_symbol.Equals(eps))
					{
						next_symbol_firsts.Add(next_symbol);
					}
					else
					{
						next_symbol_firsts.addAll(firsts[next_symbol]);
						next_symbol_firsts.remove(eps);
					}
					follows[current_symbol].addAll(next_symbol_firsts);
//                    }
				}
			}
		}
		foreach (var entry in grammars.SetOfKeyValuePairs())
		{
			Symbol non_terminal = entry.Key;
			if (!follows.ContainsKey(non_terminal))
			{
				follows[non_terminal] = new HashSet<Symbol>();
				if (non_terminal.Equals(start_symbol))
				{
					follows[non_terminal].Add(eps);
				}
			}
			ISet<Production> production_set = entry.Value;
			foreach (Production prod in production_set)
			{
				for (int i = 0; i < prod.symbolList.Count; i++)
				{
					Symbol current_symbol = prod.symbolList[i];
//                    if (!current_symbol.isTerminal) {
					if (!follows.ContainsKey(current_symbol))
					{
						follows[current_symbol] = new HashSet<Symbol>();
						if (current_symbol.Equals(start_symbol))
						{
							follows[current_symbol].Add(eps);
						}
					}
					ISet<Symbol> next_symbol_firsts = null;
					if (i != prod.symbolList.Count - 1)
					{
						Symbol next_symbol = prod.symbolList[i + 1];
						if (next_symbol.Terminal)
						{
							next_symbol_firsts = new HashSet<Symbol>();
							next_symbol_firsts.Add(next_symbol);
						}
						else
						{
							next_symbol_firsts = firsts[next_symbol];
						}
					}
					if (i == prod.symbolList.Count - 1 || next_symbol_firsts.Contains(eps))
					{
						follows[current_symbol].addAll(follows[non_terminal]);
					}
//                    }
				}
			}
		}
	}

	private void fillMatrix()
	{
		Symbol eps = new Symbol(true, "");
		symbol_matrix = new Dictionary<Symbol, IDictionary<Symbol, Production>>();
		foreach (Symbol non_terminal in non_terminal_alphabet)
		{
			symbol_matrix[non_terminal] = new Dictionary<Symbol, Production>();
		}
		foreach (var entry in grammars.SetOfKeyValuePairs())
		{
			Symbol non_terminal = entry.Key;
			ISet<Production> productions = entry.Value;
			foreach (Production production in productions)
			{
				foreach (Symbol terminal in getFirst(production.first()))
				{
					symbol_matrix[non_terminal][terminal] = production;
					if (production.symbolList.Contains(eps))
					{
						foreach (Symbol b in follows[non_terminal])
						{
							symbol_matrix[non_terminal][b] = production;
						}
					}
				}
			}
			foreach (Symbol follow in follows[non_terminal])
			{
				if (symbol_matrix[non_terminal][follow] == null)
				{
					symbol_matrix[non_terminal][follow] = new Production(true);
				}
			}
		}
	}

	private void fill_synchro()
	{
		synchro_map = new Dictionary<Symbol, ISet<Symbol>>();
		foreach (Symbol non_ter in non_terminal_alphabet)
		{
			synchro_map[non_ter] = new HashSet<Symbol>();
			synchro_map[non_ter].addAll(follows[non_ter]);
			synchro_map[non_ter].addAll(firsts[non_ter]);
		}
	}

	internal virtual Symbol get_terminal_from_str(string str)
	{
		Symbol terminal = null;
		foreach (Symbol symbol in terminal_alphabet)
		{
			if (str.StartsWith(symbol.value, StringComparison.Ordinal) && !symbol.value.Equals(""))
			{
				if (terminal == null || symbol.value.Length > terminal.value.Length)
				{
					terminal = symbol;
				}
			}
		}
		return terminal;
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void call(String analyzed_file) throws IOException
	public virtual void call(string analyzed_file)
	{
		PrintWriter writer = new PrintWriter("log.csv", StandardCharsets.UTF_8);
		writer.println("Cтек,Вход,Примечание");
		Path path = Paths.get(analyzed_file);
		string file_data = String.join("", Files.readAllLines(path));
		Stack<Symbol> stack = new Stack<Symbol>();
		stack.Push(start_symbol);
		Symbol terminal = null;
		string log_state;
		while (stack.Count > 0)
		{
		   log_state = "\"" + print_symbol_stack(stack) + "\",\"" + file_data + "\"";
		   terminal = get_terminal_from_str(file_data);
			if (terminal != null)
			{
				if (stack.Peek().isTerminal && stack.Peek().Equals(terminal))
				{
					stack.Pop();
					file_data = file_data.Substring(terminal.value.Length);
					writer.println(log_state);
				}
				else
				{
					if (stack.Peek().isTerminal && !stack.Peek().Equals(terminal))
					{
						log_state += ",\"" + "Ошибка (Несоответствие терминалов)\"";
						writer.println(log_state);
						Console.WriteLine(file_data + "->expected '" + stack.Peek().value + "' " + "Stack:" + print_symbol_stack(stack));
						stack.Pop();
						continue;
					}
					Production matrix_cell = symbol_matrix[stack.Peek()][terminal];
					if (matrix_cell == null)
					{
						bool can_to_eps = false;
						foreach (Production pr in grammars[stack.Peek()])
						{
							if (pr.Eps)
							{
								can_to_eps = true;
							}
						}
						log_state += ",\"Ошибка: ";
						if (can_to_eps)
						{
							log_state += "снимаем " + stack.Peek().ToString() + " со стека\"";
							writer.println(log_state);
							stack.Pop();
						}
						else
						{
							while (!synchro_map[stack.Peek()].Contains(terminal))
							{
								log_state += "Удаляем со входного потока:" + terminal.value + "\"";
								writer.println(log_state);
								print_error(file_data, terminal.value, stack);
								file_data = file_data.Substring(terminal.value.Length);
								terminal = get_terminal_from_str(file_data);
								log_state = "\"" + print_symbol_stack(stack) + "\",\"" + file_data + "\",\"";
							}
						}
					}
					else
					{
						if (matrix_cell.synchro_error)
						{
							if (stack.Count == 1)
							{
								log_state += ",\"Ошибка: Удаляем со входного потока " + terminal.value + "\"";
								writer.println(log_state);
								print_error(file_data, terminal.value, stack);
								file_data = file_data.Substring(terminal.value.Length);
							}
							else
							{
								log_state += ",\"СинхроОшибка: снимаем " + stack.Peek().ToString() + " со стека\"";
								writer.println(log_state);
								print_error(file_data, terminal.value, stack);
								stack.Pop();
							}
						}
						else
						{ //все ок
							writer.println(log_state);
							stack.Pop();
							if (!matrix_cell.Eps)
							{
								IEnumerator<Symbol> it = matrix_cell.symbolList.listIterator(matrix_cell.symbolList.Count);
								while (it.hasPrevious())
								{
									stack.Push(it.previous());
								}
							}
						}
					}
				}
			}
			else
			{ // terminal == null
				if (file_data.Length > 0)
				{
					log_state += ",\"Ошибка:Неопознанный символ.Удаляем со входного потока " + file_data.Substring(0, 1) + "\"";
					writer.println(log_state);
					print_error(file_data, file_data.Substring(0, 1), stack);
					file_data = file_data.Substring(1);
				}
				else
				{
					writer.println(log_state);
					stack.Pop();
				}
			}
		}
		writer.close();
	}

	private void print_error(string file, string top_file, Stack<Symbol> stack)
	{
		Console.WriteLine(file + "->Unexpected '" + top_file + "' " + "Stack:" + print_symbol_stack(stack));
	}

	private string print_symbol_stack(Stack<Symbol> stack)
	{
		Stack<Symbol> printable = new Stack<Symbol>();
		printable.addAll(stack);
		StringBuilder ret_val = new StringBuilder();
		while (printable.Count > 0)
		{
			ret_val.Append(printable.Pop());
		}
		return ret_val.ToString();
	}
}
