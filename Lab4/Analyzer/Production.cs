using System.Collections.Generic;
using System.Text;

public class Production
{
	internal IList<Symbol> symbolList;
	internal bool synchro_error = false;


	public Production()
	{
		symbolList = new List<Symbol>();
	}

	public Production(bool synchro_error)
	{
		this.synchro_error = true;
	}

	public virtual void addSymbolToProduction(Symbol symbol)
	{
		symbolList.Add(symbol);
	}

	public override string ToString()
	{
		if (synchro_error)
		{
			return "SYNCHRO_ERROR";
		}
		StringBuilder retVal = new StringBuilder();
		foreach (Symbol symbol in symbolList)
		{
			retVal.Append(symbol.ToString());
		}
		return retVal.ToString();
	}

	public virtual Symbol Eps
	{
		get
		{
			if (symbolList.Count != 1)
			{
				return null;
			}
			Symbol eps = first();
			if (eps.value.Equals(""))
			{
				return eps;
			}
			return null;
		}
	}

	public virtual bool Eps
	{
		get
		{
		   return Eps != null;
		}
	}

	public virtual Symbol first()
	{
		return symbolList[0];
	}
}
