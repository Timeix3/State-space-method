using Newtonsoft.Json;

[JsonObject]
internal class Vector
{
    [JsonProperty]
    private double[] vector;

    [JsonIgnore]
    public int Size => vector.Length;
    
    public Vector(int size) => vector = new double[size];

    [JsonConstructor]
    private Vector(double[] vector) => this.vector = vector;

    public double this[int index]
    {
        get => vector[index];
        set => vector[index] = value;
    }

    public double[] ToArray() => vector;

    public static Vector operator +(Vector v1, Vector v2)
    {
        if (v1.Size != v2.Size)
            throw new ArgumentException("Векторы должны иметь одинаковый размер для сложения");
        Vector result = new Vector(v1.Size);
        for (int i = 0; i < v1.Size; i++)
        {
            result[i] = v1[i] + v2[i];
        }
        return result;
    }

    public static Vector operator *(double d, Vector v)
    {
        Vector result = new Vector(v.Size);
        for (int i = 0; i < v.Size; i++)
        {
            result[i] = d * v[i];
        }
        return result;
    }
}


