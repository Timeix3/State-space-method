using Newtonsoft.Json;

internal class Branch
{
    [JsonProperty] public string Name { get; }
    [JsonProperty] public string Type { get; } // R, L, C, V, I
    [JsonProperty] public int From { get; }
    [JsonProperty] public int To { get; }
    [JsonProperty] public double Value { get; }

    [JsonConstructor]
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
