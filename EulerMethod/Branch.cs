using Newtonsoft.Json;

internal class Branch
{
    [JsonProperty] public string Name { get; }
    public string Parent { get; set; }
    [JsonProperty] public string Type { get; }
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
