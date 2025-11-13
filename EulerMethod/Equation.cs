internal class Equation
{
    public List<Term> Terms { get; } = new();

    public void AddTerm(Term term)
    {
        var existingTerm = Terms.FirstOrDefault(t => t.Name == term.Name);
        if (existingTerm != null) existingTerm.Coeff += term.Coeff;
        else Terms.Add(term);
    }

    public override string ToString()
    {
        var parts = Terms.Select(kv => $"{kv.ToString()}");
        return string.Join(" ", parts);
    }
}
