internal static class MMatrixBuilder
{
    public static Matrix Build(CircuitGraph graph, List<int> treeBranches)
    {
        var treeOrder = new List<int>(treeBranches);

        var allBranches = Enumerable.Range(0, graph.Branches.Count).ToList();
        var chords = allBranches.Except(treeBranches).ToList();

        chords = chords
            .OrderBy(i => TypePriority(graph.Branches[i].Type))
            .ToList();

        int nChords = chords.Count;
        int nTree = treeOrder.Count;
        Matrix M = new(nChords, nTree);

        var treeAdj = new Dictionary<int, List<(int neighbor, int branchIndex, bool direction)>>();

        foreach (int bi in treeOrder)
        {
            var b = graph.Branches[bi];
            if (!treeAdj.ContainsKey(b.From)) treeAdj[b.From] = new();
            if (!treeAdj.ContainsKey(b.To)) treeAdj[b.To] = new();
            treeAdj[b.From].Add((b.To, bi, true));
            treeAdj[b.To].Add((b.From, bi, false));
        }

        for (int i = 0; i < nChords; i++)
        {
            var chordIndex = chords[i];
            var chord = graph.Branches[chordIndex];
            int start = chord.From;
            int end = chord.To;

            var path = FindPath(treeAdj, start, end);

            for (int j = 0; j < nTree; j++)
            {
                int treeBranchIndex = treeOrder[j];
                var found = path.FirstOrDefault(p => p.branchIndex == treeBranchIndex);
                if (found.branchIndex == 0 && path.All(p => p.branchIndex != treeBranchIndex))
                    continue;
                M[i][j] = found.direction ? 1 : -1;
            }
        }

        Console.WriteLine("Порядок столбцов (ветви дерева): " +
            string.Join(", ", treeOrder.Select(i => graph.Branches[i].Name + "(" + graph.Branches[i].Type + ")")));
        Console.WriteLine("Порядок строк (хорды): " +
            string.Join(", ", chords.Select(i => graph.Branches[i].Name + "(" + graph.Branches[i].Type + ")")));

        return M;
    }

    private static int TypePriority(string type)
    {
        return type switch
        {
            "R" => 1,
            "L" => 2,
            "I" => 3,
            _ => 9
        };
    }

    private static List<(int branchIndex, bool direction)> FindPath(
        Dictionary<int, List<(int neighbor, int branchIndex, bool direction)>> adj,
        int start, int goal)
    {
        var stack = new Stack<(int node, List<(int branchIndex, bool direction)> path)>();
        stack.Push((start, new List<(int branchIndex, bool direction)>()));
        var visited = new HashSet<int>();

        while (stack.Count > 0)
        {
            var (node, path) = stack.Pop();
            if (node == goal)
                return path;
            visited.Add(node);
            if (!adj.ContainsKey(node)) continue;

            foreach (var (neighbor, branchIndex, direction) in adj[node])
            {
                if (visited.Contains(neighbor)) continue;
                var newPath = new List<(int branchIndex, bool direction)>(path)
                {
                    (branchIndex, direction)
                };
                stack.Push((neighbor, newPath));
            }
        }
        return new List<(int branchIndex, bool direction)>();
    }
}
