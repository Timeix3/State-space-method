using Newtonsoft.Json;

class Program
{
    private static void Main()
    {
        CircuitGraph graph = new();

        string path = "../../../circuit.json";
        if (!File.Exists(path))
        {
            Console.WriteLine("Файл не найден!");
            return;
        }

        graph = LoadGraphFromFile(path);
        Console.WriteLine("\nПрочитанные ветви схемы:");
        foreach (var b in graph.Branches)
            Console.WriteLine($"{b.Name} ({b.Type}) {b.From}->{b.To}, Value = {b.Value}");

        SystemData data = SystemOfEquations(graph);
        EulerSolver solver = new EulerSolver();
        Vector y = solver.Solve(data);
        Console.WriteLine("Y="); y.Print();
        new Drawer("../../../../Images/outputY.png").DrawToFile(solver.Ydata, solver.time, variablesY);
        new Drawer("../../../../Images/outputX.png").DrawToFile(solver.Xdata, solver.time, data.VariablesX.ToArray());
    }
    private static string[] variablesY;
    private static SystemData SystemOfEquations(CircuitGraph graph)
    {
        var treeBranches = TreeBuilder.BuildTree(graph);

        Console.WriteLine("Дерево содержит ветви:");
        foreach (var i in treeBranches)
            Console.WriteLine($"{graph.Branches[i].Name} ({graph.Branches[i].Type})");
        Console.WriteLine();

        Matrix M = MMatrixBuilder.Build(graph, treeBranches);
        Console.WriteLine("\nM матрица: \n");
        M.Print();

        var allBranches = Enumerable.Range(0, graph.Branches.Count).ToList();
        var chordBranchesOrdered = allBranches.Except(treeBranches)
            .OrderBy(i => MMatrixBuilder.TypePriority(graph.Branches[i].Type))
            .ToList();

        var kvl = KirchhoffBuilder.BuildKVL(M, graph, treeBranches, chordBranchesOrdered);
        var kcl = KirchhoffBuilder.BuildKCL(M, graph, treeBranches, chordBranchesOrdered);

        Console.WriteLine("\nУравнения KVL: ");
        for (int i = 0; i < kvl.Count; i++)
            Console.WriteLine($"Контур {i + 1} ({graph.Branches[chordBranchesOrdered[i]].Name}):  {kvl[i]}");

        Console.WriteLine("\nУравнения KCL: ");
        for (int j = 0; j < kcl.Count; j++)
            Console.WriteLine($"Сечение {j + 1} ({graph.Branches[treeBranches[j]].Name}):  {kcl[j]}");

        Console.WriteLine("\nЗаконы ома для резисторов: ");
        var ohm = KirchhoffBuilder.OhmLawForResistors(graph);
        for (int j = 0; j < ohm.Count; j++)
            Console.WriteLine($"{ohm[j]}");

        string[] variables = CreateVariables(graph);
        Console.WriteLine("Переменные: " + string.Join(" ", variables));
        Console.WriteLine("\nМатрица системы: ");
        Matrix systemMatrix = KirchhoffBuilder.BuildSystemMatrix(kvl, kcl, ohm, variables);
        systemMatrix.Print();

        Console.Write("Введите анализируемые переменные через пробел: ");
        variablesY = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        SystemSolver systemSolver = new(graph.Branches, systemMatrix, variables, variablesY);
        systemSolver.Solve();
        SystemData data = systemSolver.data;
        Console.WriteLine();
        data.Print();

        return data;
    }

    private static string[] CreateVariables(CircuitGraph graph)
    {
        string[] variables = new string[2 * graph.Branches.Count];
        for (int i = 0; i < graph.Branches.Count; i++)
        {
            variables[2 * i] = "U_" + graph.Branches[i].Name;
            variables[2 * i + 1] = "I_" + graph.Branches[i].Name;
        }

        return variables;
    }

    private static CircuitGraph LoadGraphFromFile(string path)
    {
        string json = File.ReadAllText(path);
        var branches = JsonConvert.DeserializeObject<List<Branch>>(json);
        CircuitGraph graph = new CircuitGraph();
        foreach (var b in branches)
            graph.AddBranch(b.Name, b.Type, b.From, b.To, b.Value);
        return graph;
    }
}
