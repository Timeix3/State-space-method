public class Term(string name, TermType type, double coeff)
{
    public string Name { get; } = name;
    public TermType Type { get; } = type;
    public double Coeff { get; set; } = coeff;
    public override string ToString()
    {
        if (Type == TermType.Equal) return Name;
        else if (Coeff == 1 || Coeff == -1) return (Coeff > 0 ? "+" : "-") + Name;
        else return (Coeff > 0 ? "+" : "") + coeff + "*" + Name;
    }
}

public enum TermType
{
    Equal, Variable
}
