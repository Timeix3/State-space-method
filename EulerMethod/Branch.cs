internal class Branch
{
    public string Name { get; }
    public string Type { get; } // R, L, C, V, I
    public int From { get; }
    public int To { get; }
    public double Value { get; }

    public Branch(string name, string type, int from, int to, double value)
    {
        Name = name;
        Type = type;
        From = from;
        To = to;
        Value = value;
    }
}

internal class CircuitGraph
{
    public List<Branch> Branches { get; } = new();
    public void AddBranch(string name, string type, int from, int to, double value)
        => Branches.Add(new Branch(name, type, from, to, value));
}
