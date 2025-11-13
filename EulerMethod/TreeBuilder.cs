internal static class TreeBuilder
{
    public static List<int> BuildTree(CircuitGraph graph)
    {
        var tree = new List<int>();
        var uf = new UnionFind(graph.Branches.SelectMany(b => new[] { b.From, b.To }).Max() + 1);

        AddBranches(graph, tree, uf, "V");
        AddBranches(graph, tree, uf, "C");
        AddBranches(graph, tree, uf, "R");

        return tree;
    }

    private static void AddBranches(CircuitGraph g, List<int> tree, UnionFind uf, string type)
    {
        foreach (var (b, idx) in g.Branches.Select((b, i) => (b, i)))
        {
            if (b.Type != type) continue;
            if (!uf.Connected(b.From, b.To))
            {
                uf.Union(b.From, b.To);
                tree.Add(idx);
                if (uf.ComponentsCount == 1)
                    break;
            }
        }
    }
}
