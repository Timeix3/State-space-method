internal class CircuitGraph
{
    public List<Branch> Branches { get; } = new();
    public void AddBranch(string name, string type, int from, int to, double value)
        => Branches.Add(new Branch(name, type, from, to, value));
}
