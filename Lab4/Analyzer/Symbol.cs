public class Symbol
{
	internal bool isTerminal;
	internal string value;

	public virtual string Value
	{
		get
		{
			return value;
		}
	}

	public virtual bool Terminal
	{
		get
		{
			return isTerminal;
		}
	}

	public Symbol(bool isTerminal, string value)
	{
		this.isTerminal = isTerminal;
		this.value = value;
	}

	public override string ToString()
	{
		if (!isTerminal)
		{
			return "<" + value + ">";
		}
		if (value.Equals(""))
		{
			return "#eps#";
		}
		return value;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
			return false;
		}
		Symbol symbol = (Symbol) o;
		return isTerminal == symbol.isTerminal && value.Equals(symbol.value);
	}

	public override int GetHashCode()
	{
		return Objects.hash(isTerminal, value);
	}
}
