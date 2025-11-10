internal class Variable
{
    public string Name { get; }
    public Variable(string name) => Name = name;
    public override string ToString() => Name;
}

internal class Equation
{
    public Dictionary<string, double> Terms { get; } = new();
    public double Constant { get; set; } = 0.0;

    public void AddTerm(string name, double coeff)
    {
        if (Terms.ContainsKey(name)) Terms[name] += coeff;
        else Terms[name] = coeff;
    }

    public override string ToString()
    {
        var parts = Terms.Select(kv => $"{(kv.Value >= 0 ? "+" : "")}{kv.Value}*{kv.Key}");
        return string.Join(" ", parts) + $" = {Constant}";
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
            eq.AddTerm($"U_{chord.Name}", +1);

            for (int col = 0; col < treeBranches.Count; col++)
            {
                double coeff = M[row][col];
                if (Math.Abs(coeff) > 1e-9)
                {
                    var treeBranch = graph.Branches[treeBranches[col]];
                    eq.AddTerm($"U_{treeBranch.Name}", coeff);
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

        Matrix Mt = Transpose(M);

        for (int col = 0; col < treeBranches.Count; col++)
        {
            var treeBranchIndex = treeBranches[col];
            var treeBranch = graph.Branches[treeBranchIndex];

            Equation eq = new Equation();
            eq.AddTerm($"I_{treeBranch.Name}", 1);

            for (int row = 0; row < chordBranchesOrdered.Count; row++)
            {
                double coeff = Mt[col][row];
                if (Math.Abs(coeff) > 1e-9)
                {
                    var chord = graph.Branches[chordBranchesOrdered[row]];
                    eq.AddTerm($"I_{chord.Name}", -coeff);
                }
            }

            equations.Add(eq);
        }

        return equations;
    }

    private static Matrix Transpose(Matrix M)
    {
        Matrix T = new Matrix(M.Columns, M.Rows);
        for (int i = 0; i < M.Rows; i++)
            for (int j = 0; j < M.Columns; j++)
                T[j][i] = M[i][j];
        return T;
    }
}
