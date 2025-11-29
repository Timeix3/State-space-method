
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
            eq.AddTerm(new Term($"U_{chord.Name}",TermType.Variable, chord.Type == "V" ? -1 : 1));
            for (int col = 0; col < treeBranches.Count; col++)
            {
                double coeff = M[row][col];
                if (Math.Abs(coeff) > 1e-10)
                {
                    var treeBranch = graph.Branches[treeBranches[col]];
                    eq.AddTerm(new Term($"U_{treeBranch.Name}", TermType.Variable, (treeBranch.Type == "V" ? -1 : 1) * coeff));
                }
            }
            eq.AddTerm(new Term("= 0", TermType.Equal, -1));
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
            eq.AddTerm(new Term($"I_{treeBranch.Name}", TermType.Variable, 1));
            for (int row = 0; row < chordBranchesOrdered.Count; row++)
            {
                double coeff = Mt[col][row];
                if (Math.Abs(coeff) > 1e-10)
                {
                    var chord = graph.Branches[chordBranchesOrdered[row]];
                    eq.AddTerm(new Term($"I_{chord.Name}", TermType.Variable, -coeff));
                }
            }
            eq.AddTerm(new Term("= 0", TermType.Equal, -1));
            equations.Add(eq);
        }

        return equations;
    }

    public static List<Equation> OhmLawForResistors(CircuitGraph graph)
    {
        var equations = new List<Equation>();
        foreach(var branch in graph.Branches)
        {
            if (branch.Type == "R")
            {
                Equation eq = new Equation();
                eq.AddTerm(new Term($"U_{branch.Name}", TermType.Variable, 1));
                eq.AddTerm(new Term($"I_{branch.Name}", TermType.Variable, -branch.Value));
                eq.AddTerm(new Term("= 0", TermType.Equal, -1));
                equations.Add(eq);
            }
        }
        return equations;
    }

    public static Matrix BuildSystemMatrix(List<Equation> kvl, List<Equation> kcl, List<Equation> ohm, string[] variables)
    {
        Matrix matrix = new Matrix(kvl.Count + kcl.Count + ohm.Count, variables.Length);
        List<Equation> allEquations = [.. kvl, .. kcl, .. ohm];
        for (int i = 0; i < allEquations.Count;i++)
        {
            int j = 0;
            while (allEquations[i].Terms[j].Type != TermType.Equal)
            {
                matrix[i][Array.IndexOf(variables, allEquations[i].Terms[j].Name)] = allEquations[i].Terms[j].Coeff;
                j++;
            }
        }
        return matrix;
    }
}