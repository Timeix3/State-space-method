
public enum TermType
{
    Derivative,
    Equal,
    Variable
}

public class Term(string name, TermType type, double coeff, double value)
{
    public string Name { get; } = name;
    public TermType Type { get; } = type;
    public double Coeff { get; set; } = coeff;
    public double Value { get; } = value;
    public override string ToString()
    {
        if (Type == TermType.Equal) return Name;
        else if (Coeff == 1 || Coeff == -1) return (Coeff > 0 ? "+" : "-") + Name;
        else return (Coeff > 0 ? "+" : "") + coeff + "*" + Name;
    }
}

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

internal static class KirchhoffBuilder
{
    public static List<Equation> BuildKVL(
        Matrix M,
        CircuitGraph graph,
        List<int> treeBranches,
        List<int> chordBranchesOrdered)
    {
        var equations = new List<Equation>();

        for (int row = 0; row < chordBranchesOrdered.Count; row++)
        {
            var chordIndex = chordBranchesOrdered[row];
            var chord = graph.Branches[chordIndex];

            Equation eq = new Equation();
            eq.AddTerm(new Term($"U_{chord.Name}",TermType.Variable, 1, chord.Value));
            eq.AddTerm(new Term("=", TermType.Equal, -1, 0));
            for (int col = 0; col < treeBranches.Count; col++)
            {
                double coeff = M[row][col];
                if (Math.Abs(coeff) > 1e-9)
                {
                    var treeBranch = graph.Branches[treeBranches[col]];
                    eq.AddTerm(new Term($"U_{treeBranch.Name}", TermType.Variable, -coeff, chord.Value));
                }
            }

            equations.Add(eq);
        }

        return equations;
    }

    public static List<Equation> BuildKCL(
        Matrix M,
        CircuitGraph graph,
        List<int> treeBranches,
        List<int> chordBranchesOrdered)
    {
        var equations = new List<Equation>();

        Matrix Mt = Matrix.Transpose(M);

        for (int col = 0; col < treeBranches.Count; col++)
        {
            var treeBranchIndex = treeBranches[col];
            var treeBranch = graph.Branches[treeBranchIndex];

            Equation eq = new Equation();
            eq.AddTerm(new Term($"I_{treeBranch.Name}", TermType.Variable, 1, treeBranch.Value));
            eq.AddTerm(new Term("=", TermType.Equal, -1, 0));
            for (int row = 0; row < chordBranchesOrdered.Count; row++)
            {
                double coeff = Mt[col][row];
                if (Math.Abs(coeff) > 1e-9)
                {
                    var chord = graph.Branches[chordBranchesOrdered[row]];
                    eq.AddTerm(new Term($"I_{chord.Name}", TermType.Variable, coeff, chord.Value));
                }
            }

            equations.Add(eq);
        }

        return equations;
    }
}
