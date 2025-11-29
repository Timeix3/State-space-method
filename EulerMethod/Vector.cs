
internal class Vector
{
    private List<double> _vector;

    public int Size => _vector.Count;
    
    public Vector(int size) => _vector = [.. new double[size]];

    public double this[int index]
    {
        get => _vector[index];
        set => _vector[index] = value;
    }

    public double[] ToArray() => _vector.ToArray();

    public void Print() => Console.WriteLine("(" + string.Join(" ", _vector) + ")");

    public void RemoveAt(int index) => _vector.RemoveAt(index);

    public void Add(double value) => _vector.Add(value);

    public void Normalize(double value)
    {
        for (int i = 0; i < Size; i++)
            _vector[i] /= value;
    }

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


