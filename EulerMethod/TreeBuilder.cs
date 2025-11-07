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

internal class UnionFind
{
    private int[] parent;
    private int[] rank;
    public int ComponentsCount { get; private set; }

    public UnionFind(int n)
    {
        parent = Enumerable.Range(0, n).ToArray();
        rank = new int[n];
        ComponentsCount = n;
    }

    private int Find(int x)
    {
        if (parent[x] != x) parent[x] = Find(parent[x]);
        return parent[x];
    }

    public bool Connected(int a, int b) => Find(a) == Find(b);

    public void Union(int a, int b)
    {
        a = Find(a); b = Find(b);
        if (a == b) return;
        if (rank[a] < rank[b]) parent[a] = b;
        else if (rank[a] > rank[b]) parent[b] = a;
        else { parent[b] = a; rank[a]++; }
        ComponentsCount--;
    }
}
