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
